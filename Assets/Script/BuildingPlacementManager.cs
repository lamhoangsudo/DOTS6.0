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
        if (Input.GetMouseButtonDown(0) && building.buildingType != BuildingTypeSO.BuildingType.None && ResourceManager.Instance.HasEnoughResource(building.resourceCost))
        {
            Vector3 pos = MouseWorldPositionManager.mouseWorldPositionManager.GetMousePosition();
            if (!CanPlaceBuilding(pos)) return;
            ResourceManager.Instance.SpendResourceAmount(building.resourceCost);
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityReferenecs entityReferenecs = entityManager.CreateEntityQuery(typeof(EntityReferenecs)).GetSingleton<EntityReferenecs>();
            //Entity buildingContructionEntityVisual = entityManager.Instantiate(building.GetPrefabEntity(entityReferenecs));
            //entityManager.SetComponentData<LocalTransform>(buildingContructionEntityVisual, LocalTransform.FromPosition(pos));

            Entity buildingContructionEntityVisual = entityManager.Instantiate(building.GetVisualPrefabEntity(entityReferenecs));
            entityManager.SetComponentData<LocalTransform>(buildingContructionEntityVisual, LocalTransform.FromPosition(new float3(pos.x, pos.y + building.constructionYOffset, pos.z)));

            Entity buildingContructionEntity = entityManager.Instantiate(entityReferenecs.buildingContructionPrefabEntity);
            entityManager.SetComponentData<LocalTransform>(buildingContructionEntity, LocalTransform.FromPosition(pos));
            entityManager.SetComponentData(buildingContructionEntity, new BuildingContruction
            {
                buildingType = building.buildingType,
                progress = 0f,
                maxProgress = building.constructionTimeMax,
                finalPrefabEntity = building.GetPrefabEntity(entityReferenecs),
                visualEntity = buildingContructionEntityVisual,
                startPosition = new float3(pos.x, pos.y + building.constructionYOffset, pos.z),
                endPosition = pos,
            });
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
            CollidesWith = 1u << GameAssets.BUILDINGS_LAYER | 1u << GameAssets.RESOURCE_NODE,
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
            //hit something with the same building type
            foreach (DistanceHit distanceHit in distanceHits)
            {
                if (entityManager.HasComponent<BuildingTypeSOHolder>(distanceHit.Entity))
                {
                    if (entityManager.GetComponentData<BuildingTypeSOHolder>(distanceHit.Entity).buildingTypeSO == building.buildingType)
                    {
                        //same building type too close
                        return false;
                    }
                }
                if (entityManager.HasComponent<BuildingContruction>(distanceHit.Entity))
                {
                    if (entityManager.GetComponentData<BuildingContruction>(distanceHit.Entity).buildingType == building.buildingType)
                    {
                        //same contruction building type too close
                        return false;
                    }
                }
            }
        }
        distanceHits.Clear();
        if (building is BuildingResourceHarversterTypeSO buildingResourceHarversterTypeSO)
        {
            bool hasValidNearbyResource = false;
            if (collisionWorld.OverlapSphere(pos, buildingResourceHarversterTypeSO.harverstDistance, ref distanceHits, collisionFilter))
            {
                //hit something with the same resource type
                foreach (DistanceHit distanceHit in distanceHits)
                {
                    if (entityManager.HasComponent<ResourceTypeSOHolder>(distanceHit.Entity))
                    {
                        if (entityManager.GetComponentData<ResourceTypeSOHolder>(distanceHit.Entity).resourceType == buildingResourceHarversterTypeSO.resourceType)
                        {
                            hasValidNearbyResource = true;
                            break;
                        }
                    }
                }
            }
            if (!hasValidNearbyResource)
            {
                return false;
            }
        }
        distanceHits.Clear();
        distanceHits.Dispose();
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
        foreach (MeshRenderer meshRenderer in ghost.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = material;
        }
        OnSelectedBuildingTypeSOChanged?.Invoke(this, EventArgs.Empty);
    }
}
