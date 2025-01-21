using System;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }
    public event EventHandler OnSelectAreaStart;
    public event EventHandler OnSelectAreaEnd;
    private Vector2 selectStartMousePosition;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectStartMousePosition = Input.mousePosition;
            OnSelectAreaStart?.Invoke(this, EventArgs.Empty);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //almost like database query
            //entity manager system
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //build query
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Select>().Build(entityManager);
            //get result
            //get array entity
            NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
            //get array component
            NativeArray<Select> selectArray = entityQuery.ToComponentDataArray<Select>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (entityManager.IsComponentEnabled<Select>(entities[i]))
                {
                    entityManager.SetComponentEnabled<Select>(entities[i], false);
                    Select select = selectArray[i];
                    //select.OnDeSelect = true;
                    //select.OnSelect = false;
                    select.SetSelect(false);
                    selectArray[i] = select;
                    entityManager.SetComponentData<Select>(entities[i], select);
                }
                //when run this code
                //DOTS will run query again 
                //but we modifile component before
                //then query return 0
                //entityQuery.CopyFromComponentDataArray(selectArray);
            }
            Rect selectionAreaRect = GetSelectAreaRect();
            float size = selectionAreaRect.width + selectionAreaRect.height;
            if (size < 40f)
            {
                //single select
                //another way to build query
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                //another way to get result
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                RaycastInput raycastInput = new()
                {
                    Start = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(0),
                    End = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(999f),
                    Filter = new CollisionFilter()
                    {
                        //...11111111
                        //belong to all layers
                        BelongsTo = ~0u,
                        //...00000001
                        //...00001000
                        //only affects layer 6
                        CollidesWith = 1u << GameAssets.UNIT_LAYER,
                        GroupIndex = 0,
                    }
                };
                if(collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
                {
                    if(entityManager.HasComponent<Unit>(raycastHit.Entity) 
                        && entityManager.HasComponent<Select>(raycastHit.Entity)) {
                        //hit a unit
                        entityManager.SetComponentEnabled<Select>(raycastHit.Entity, true);
                        Select select = entityManager.GetComponentData<Select>(raycastHit.Entity);
                        //select.OnSelect = true;
                        //select.OnDeSelect = false;
                        select.SetSelect(true);
                        entityManager.SetComponentData<Select>(raycastHit.Entity, select);
                    }
                }

            }
            else
            {
                //mutile select
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Select>().Build(entityManager);
                entities = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> unitLocalTransforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                //selectArray = entityQuery.ToComponentDataArray<Select>(Allocator.Temp);
                for (int i = 0; i < unitLocalTransforms.Length; i++)
                {
                    LocalTransform unitLocalTranform = unitLocalTransforms[i];
                    Vector2 unitScenePosition = Camera.main.WorldToScreenPoint(unitLocalTranform.Position);
                    if (selectionAreaRect.Contains(unitScenePosition))
                    {
                        //unit is in select area
                        entityManager.SetComponentEnabled<Select>(entities[i], true);
                        Select select = entityManager.GetComponentData<Select>(entities[i]);
                        //select.OnSelect = true;
                        //select.OnDeSelect = false;
                        select.SetSelect(true);
                        entityManager.SetComponentData(entities[i], select);
                    }
                }
                //entityQuery.CopyFromComponentDataArray<Select>(selectArray);
            }
            OnSelectAreaEnd?.Invoke(this, EventArgs.Empty);
        }
        if (Input.GetMouseButtonDown(1))
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Select>().Build(entityManager);
            NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<UnitMover> unitMovers = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            NativeArray<float3> positionArray = GenerateMovePositionArray(MouseWorldPositionManager.mouseWorldPositionManager.GetMousePosition(), unitMovers.Length);
            for (int i = 0; i < unitMovers.Length; i++)
            {
                UnitMover unitMover = unitMovers[i];
                unitMover.movePosition = positionArray[i];
                unitMovers[i] = unitMover;
            }
            entityQuery.CopyFromComponentDataArray<UnitMover>(unitMovers);
        }
    }
    public Rect GetSelectAreaRect()
    {
        Vector2 selectEndMousePosition = Input.mousePosition;
        Vector2 lowerLeftConer = new Vector2
                (
                Mathf.Min(selectStartMousePosition.x, selectEndMousePosition.x),
                Mathf.Min(selectStartMousePosition.y, selectEndMousePosition.y)
                );
        Vector2 upperRightConer = new Vector2
            (
            Mathf.Max(selectStartMousePosition.x, selectEndMousePosition.x),
            Mathf.Max(selectStartMousePosition.y, selectEndMousePosition.y)
            );
        return new(lowerLeftConer.x, lowerLeftConer.y, upperRightConer.x - lowerLeftConer.x, upperRightConer.y - lowerLeftConer.y);
    }
    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
    {
        NativeArray<float3> positionArray = new(positionCount, allocator: Allocator.Temp);
        if (positionCount <= 0)
        {
            return positionArray;
        }
        positionArray[0] = targetPosition;
        if (positionCount == 1)
        {
            return positionArray;
        }
        float ringSize = 2.2f;
        int ring = 0;
        int positionIndex = 1;
        while (positionIndex < positionCount)
        {
            int ringPositionCount = 3 + ring * 2;
            for (int i = 0; i < ringPositionCount; i++)
            {
                float angle = i * (math.PI2 / ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle), new(ringSize * (ring + 1), 0, 0));
                float3 ringPosition = targetPosition + ringVector;
                positionArray[positionIndex] = ringPosition;
                positionIndex++;
                if(positionIndex >= positionCount)
                {
                    break;
                }
            }
            ring++;
        }
        return positionArray;
    }
}
