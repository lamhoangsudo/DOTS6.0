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
using UnityEngine.EventSystems;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }
    public event EventHandler OnSelectAreaStart;
    public event EventHandler OnSelectAreaEnd;
    private Vector2 selectStartMousePosition;
    public event EventHandler OnSelectChange;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if(BuildingPlacementManager.buildingPlacementManager.GetActiveBuildingTypeSO().buildingType != BuildingTypeSO.BuildingType.None)
        {
            return;
        }
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
                //entityQueryMoveOveride.CopyFromComponentDataArray(selectArray);
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
                        CollidesWith = 1u << GameAssets.UNIT_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                        GroupIndex = 0,
                    }
                };
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
                {
                    if (entityManager.HasComponent<Select>(raycastHit.Entity))
                    {
                        //hit a selectable entity
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
                //selectArray = entityQueryMoveOveride.ToComponentDataArray<Select>(Allocator.Temp);
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
                //entityQueryMoveOveride.CopyFromComponentDataArray<Select>(selectArray);
            }
            OnSelectAreaEnd?.Invoke(this, EventArgs.Empty);
            OnSelectChange?.Invoke(this, EventArgs.Empty);
        }
        if (Input.GetMouseButtonDown(1))
        {
            bool isAttackingSingleTarget = false;
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //set target
            //another way to build query
            EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
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
                    CollidesWith = 1u << GameAssets.UNIT_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                    GroupIndex = 0,
                }
            };
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
            {
                if (entityManager.HasComponent<Faction>(raycastHit.Entity))
                {
                    //hit a unit
                    Faction factionTarget = entityManager.GetComponentData<Faction>(raycastHit.Entity);
                    if (factionTarget.factionType == FactionType.Zombie)
                    {
                        isAttackingSingleTarget = true;
                        EntityQuery entityQuerySelect = new EntityQueryBuilder(Allocator.Temp).WithAll<Select>().WithPresent<TargetOveride>().Build(entityManager);
                        NativeArray<Entity> entities = entityQuerySelect.ToEntityArray(Allocator.Temp);
                        NativeArray<TargetOveride> unitTargetOverides = entityQuerySelect.ToComponentDataArray<TargetOveride>(Allocator.Temp);
                        for (int i = 0; i < unitTargetOverides.Length; i++)
                        {
                            TargetOveride targetOveride = unitTargetOverides[i];
                            targetOveride.targetEntity = raycastHit.Entity;
                            entityManager.SetComponentEnabled<MoveOveride>(entities[i], false);
                        }
                        entityQuerySelect.CopyFromComponentDataArray<TargetOveride>(unitTargetOverides);
                    }
                    //else
                    //{
                    //    isAttackingSingleTarget = false;
                    //}
                }
            }
            if (!isAttackingSingleTarget)
            {
                EntityQuery entityQueryMoveOveride = new EntityQueryBuilder(Allocator.Temp).WithAll<Select>().WithPresent<MoveOveride, TargetOveride, TargetPositionPathQueued, FlowFieldPathRequest, FlowFieldFollower>().Build(entityManager);
                NativeArray<Entity> entities = entityQueryMoveOveride.ToEntityArray(Allocator.Temp);
                NativeArray<MoveOveride> unitMoverOverides = entityQueryMoveOveride.ToComponentDataArray<MoveOveride>(Allocator.Temp);
                NativeArray<TargetOveride> unitTargetOverides = entityQueryMoveOveride.ToComponentDataArray<TargetOveride>(Allocator.Temp);
                NativeArray<TargetPositionPathQueued> targetPositionPathQueueds = entityQueryMoveOveride.ToComponentDataArray<TargetPositionPathQueued>(Allocator.Temp);
                NativeArray<FlowFieldPathRequest> flowFieldPathRequests = entityQueryMoveOveride.ToComponentDataArray<FlowFieldPathRequest>(Allocator.Temp);
                NativeArray<FlowFieldFollower> flowFieldFollowers = entityQueryMoveOveride.ToComponentDataArray<FlowFieldFollower>(Allocator.Temp);
                NativeArray<float3> positionArray = GenerateMovePositionArray(MouseWorldPositionManager.mouseWorldPositionManager.GetMousePosition(), unitMoverOverides.Length);
                for (int i = 0; i < unitMoverOverides.Length; i++)
                {
                    TargetOveride targetOveride = unitTargetOverides[i];
                    targetOveride.targetEntity = Entity.Null;
                    unitTargetOverides[i] = targetOveride;

                    MoveOveride unitMoveOveride = unitMoverOverides[i];
                    unitMoveOveride.targetPosition = positionArray[i];
                    unitMoverOverides[i] = unitMoveOveride;
                    entityManager.SetComponentEnabled<MoveOveride>(entities[i], true);

                    TargetPositionPathQueued targetPositionPathQueued = targetPositionPathQueueds[i];
                    targetPositionPathQueued.targetPosition = positionArray[i];
                    targetPositionPathQueueds[i] = targetPositionPathQueued;
                    entityManager.SetComponentEnabled<TargetPositionPathQueued>(entities[i], true);

                    entityManager.SetComponentEnabled<FlowFieldFollower>(entities[i], false);
                    entityManager.SetComponentEnabled<FlowFieldPathRequest>(entities[i], false);
                }
                entityQueryMoveOveride.CopyFromComponentDataArray<MoveOveride>(unitMoverOverides);
                entityQueryMoveOveride.CopyFromComponentDataArray<TargetOveride>(unitTargetOverides);
                entityQueryMoveOveride.CopyFromComponentDataArray<TargetPositionPathQueued>(targetPositionPathQueueds);
            }

            //Handle barracks rally point
            EntityQuery entityQueryBarracksRallyPoint = new EntityQueryBuilder(Allocator.Temp).WithAll<Select, BuildingBarracks, LocalTransform>().Build(entityManager);
            NativeArray<BuildingBarracks> buildingBarracks = entityQueryBarracksRallyPoint.ToComponentDataArray<BuildingBarracks>(Allocator.Temp);
            NativeArray<LocalTransform> localTransforms = entityQueryBarracksRallyPoint.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            for (int i = 0; i < buildingBarracks.Length; i++)
            {
                BuildingBarracks buildingBarrack = buildingBarracks[i];
                buildingBarrack.rallyPositionOffset = (float3)MouseWorldPositionManager.mouseWorldPositionManager.GetMousePosition() - localTransforms[i].Position;
                buildingBarracks[i] = buildingBarrack;
            }
            entityQueryBarracksRallyPoint.CopyFromComponentDataArray<BuildingBarracks>(buildingBarracks);
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
                if (positionIndex >= positionCount)
                {
                    break;
                }
            }
            ring++;
        }
        return positionArray;
    }
}
