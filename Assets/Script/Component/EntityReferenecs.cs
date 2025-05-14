using Unity.Entities;

public struct EntityReferenecs : IComponentData
{
    public Entity bulletEntity, zombie, shootAttackLight, soldier, scout, buildingTowerPrefabEntity, buildingbarracksPrefabEntity;
    public Entity buildingIronMinePrefabEntity;
    public Entity buildingGoldMinePrefabEntity;
    public Entity buildingOilMinePrefabEntity;
}
