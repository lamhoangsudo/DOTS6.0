using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private BuildingTypeSO building;
    [SerializeField] private UnityEngine.Material material;
    public event EventHandler OnSelectedBuildingTypeSOChanged;
    public static BuildingPlacementManager buildingPlacementManager { get; private set; }
    private Transform ghost;
    private void Awake()
    {
        if (buildingPlacementManager == null)
        {
            buildingPlacementManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (ghost != null)
        {
            ghost.position = Vector3.Lerp(ghost.position, MouseWorldPositionManager.mouseWorldPositionManager.GetMousePosition(), Time.deltaTime * 10f);
        }
        if (EventSystem.current.IsPointerOverGameObject() || building.buildingType == BuildingTypeSO.BuildingType.None)
            return;
        if (Input.GetMouseButtonDown(1))
        {
            SetActiveBuildingTypeSO(GameAssets.instance.buildingTypeListSO.defaultBuildingTypeSO);
        }
        if (Input.GetMouseButtonDown(0) && building.buildingType != BuildingTypeSO.BuildingType.None)
        {
            Vector3 pos = MouseWorldPositionManager.mouseWorldPositionManager.GetMousePosition();
            if (!CanPlaceBuilding(pos)) return;
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityReferenecs entityReferenecs = entityManager.CreateEntityQuery(typeof(EntityReferenecs)).GetSingleton<EntityReferenecs>();
            Entity spawnedEntity = entityManager.Instantiate(building.GetPrefabEntity(entityReferenecs));
            entityManager.SetComponentData<LocalTransform>(spawnedEntity, LocalTransform.FromPosition(pos));
        }
    }
    private bool CanPlaceBuilding(Vector3 pos)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        CollisionFilter collisionFilter = new()
        {
            //...11111111
            //belong to all layers
            BelongsTo = ~0u,
            //...00000001
            //...00001000
            //only affects layer 6
            CollidesWith = 1u << GameAssets.BUILDINGS_LAYER,
            GroupIndex = 0,
        };
        UnityEngine.BoxCollider boxCollider = building.prefab.GetComponent<UnityEngine.BoxCollider>();
        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
        if (collisionWorld.OverlapBox(pos, quaternion.identity, boxCollider.size * 0.5f * 1.1f, ref distanceHits, collisionFilter))
        {
            return false;
        }
        distanceHits.Clear();
        if (collisionWorld.OverlapSphere(pos, building.buildingDistanceMin, ref distanceHits, collisionFilter))
        {
            foreach (DistanceHit distanceHit in distanceHits)
            {
                if (entityManager.HasComponent<BuildingTypeSOHolder>(distanceHit.Entity))
                {
                    if (entityManager.GetComponentData<BuildingTypeSOHolder>(distanceHit.Entity).buildingTypeSO == building.buildingType)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public BuildingTypeSO GetActiveBuildingTypeSO()
    {
        return building;
    }
    public void SetActiveBuildingTypeSO(BuildingTypeSO buildingTypeSO)
    {
        building = buildingTypeSO;
        if (ghost != null) Destroy(ghost.gameObject);
        if (building.buildGhost != null) ghost = Instantiate(building.buildGhost);
        foreach(MeshRenderer meshRenderer in ghost.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = material;
        }
        OnSelectedBuildingTypeSOChanged?.Invoke(this, EventArgs.Empty);
    }
}
