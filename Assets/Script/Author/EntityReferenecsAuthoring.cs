using Unity.Entities;
using UnityEngine;

public class EntityReferenecsAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject bullet, zombie, shootLight, soldier, scout, buildingbarracksPrefab, buildingTowerPrefab;
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
                buildingbarracksPrefabEntity = GetEntity(authoring.buildingbarracksPrefab, TransformUsageFlags.Dynamic),
                buildingTowerPrefabEntity = GetEntity(authoring.buildingTowerPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}


