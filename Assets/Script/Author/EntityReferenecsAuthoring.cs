using Unity.Entities;
using UnityEngine;

public class EntityReferenecsAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject bullet, zombie, shootLight, soldier, scout, buildingbarracksPrefab, buildingTowerPrefab, buildingIronMinePrefab, buildingGoldMinePrefab, buildingOilMinePrefab, buildingConstructionPrefab;
    [SerializeField] private GameObject buildingBarracksPrefabVisual, buildingTowerPrefabVisual, buildingIronMinePrefabvisual, buildingGoldMinePrefabVisual, buildingOilMinePrefabVisual;
    public class EntityReferenecsAuthoringBaker : Baker<EntityReferenecsAuthoring>
    {
        public override void Bake(EntityReferenecsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityReferenecs
            {
                bulletEntity = GetEntity(authoring.bullet, TransformUsageFlags.Dynamic),
                zombie = GetEntity(authoring.zombie, TransformUsageFlags.Dynamic),
                shootAttackLight = GetEntity(authoring.shootLight, TransformUsageFlags.Dynamic),
                soldier = GetEntity(authoring.soldier, TransformUsageFlags.Dynamic),
                scout = GetEntity(authoring.scout, TransformUsageFlags.Dynamic),
                buildingBarracksPrefabEntity = GetEntity(authoring.buildingbarracksPrefab, TransformUsageFlags.Dynamic),
                buildingTowerPrefabEntity = GetEntity(authoring.buildingTowerPrefab, TransformUsageFlags.Dynamic),
                buildingIronMinePrefabEntity = GetEntity(authoring.buildingIronMinePrefab, TransformUsageFlags.Dynamic),
                buildingGoldMinePrefabEntity = GetEntity(authoring.buildingGoldMinePrefab, TransformUsageFlags.Dynamic),
                buildingOilMinePrefabEntity = GetEntity(authoring.buildingOilMinePrefab, TransformUsageFlags.Dynamic),
                buildingContructionPrefabEntity = GetEntity(authoring.buildingConstructionPrefab, TransformUsageFlags.Dynamic),
                buildingVisualBarracksPrefabEntity = GetEntity(authoring.buildingBarracksPrefabVisual, TransformUsageFlags.Dynamic),
                buildingVisualTowerPrefabEntity = GetEntity(authoring.buildingTowerPrefabVisual, TransformUsageFlags.Dynamic),
                buildingVisualIronMinePrefabEntity = GetEntity(authoring.buildingIronMinePrefabvisual, TransformUsageFlags.Dynamic),
                buildingVisualGoldMinePrefabEntity = GetEntity(authoring.buildingGoldMinePrefabVisual, TransformUsageFlags.Dynamic),
                buildingVisualOilMinePrefabEntity = GetEntity(authoring.buildingOilMinePrefabVisual, TransformUsageFlags.Dynamic),
            });
        }
    }
}


