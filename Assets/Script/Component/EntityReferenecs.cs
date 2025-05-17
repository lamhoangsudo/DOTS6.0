using Unity.Entities;

public struct EntityReferenecs : IComponentData
{
    public Entity bulletEntity, zombie, shootAttackLight, soldier, scout;
    public Entity buildingTowerPrefabEntity;
    public Entity buildingBarracksPrefabEntity;
    public Entity buildingIronMinePrefabEntity;
    public Entity buildingGoldMinePrefabEntity;
    public Entity buildingOilMinePrefabEntity;
    public Entity buildingContructionPrefabEntity;
    public Entity buildingVisualTowerPrefabEntity;
    public Entity buildingVisualBarracksPrefabEntity;
    public Entity buildingVisualIronMinePrefabEntity;
    public Entity buildingVisualGoldMinePrefabEntity;
    public Entity buildingVisualOilMinePrefabEntity;
    public Entity buildingVisualContructionPrefabEntity;
}
