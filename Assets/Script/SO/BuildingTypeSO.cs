using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypeSO", menuName = "Scriptable Objects/BuildingTypeSO")]
public class BuildingTypeSO : ScriptableObject
{
    public enum BuildingType
    {
        None = 0,
        ZomebieSpawner = 1,
        Tower = 2,
        Barracks = 3,
        HQ = 4,
        GoldMine = 5,
        IronMine = 6,
        OilMine = 7,
    }
    public BuildingType buildingType;
    public float constructionTimeMax;
    public float constructionYOffset;
    public GameObject prefab;
    public float buildingDistanceMin;
    public bool showInBuildingPlacementUI;
    public Sprite icon;
    public Transform buildGhost;
    public ResourceAmount[] resourceCost;
    public Entity GetPrefabEntity(EntityReferenecs entityReferenecs)
    {
        return buildingType switch
        {
            BuildingType.Tower => entityReferenecs.buildingTowerPrefabEntity,
            BuildingType.Barracks => entityReferenecs.buildingBarracksPrefabEntity,
            BuildingType.IronMine => entityReferenecs.buildingIronMinePrefabEntity,
            BuildingType.GoldMine => entityReferenecs.buildingGoldMinePrefabEntity,
            BuildingType.OilMine => entityReferenecs.buildingOilMinePrefabEntity,
            _ => entityReferenecs.buildingTowerPrefabEntity,
        };
    }
    public Entity GetVisualPrefabEntity(EntityReferenecs entityReferenecs)
    {
        return buildingType switch
        {
            BuildingType.Tower => entityReferenecs.buildingVisualTowerPrefabEntity,
            BuildingType.Barracks => entityReferenecs.buildingVisualBarracksPrefabEntity,
            BuildingType.IronMine => entityReferenecs.buildingVisualIronMinePrefabEntity,
            BuildingType.GoldMine => entityReferenecs.buildingVisualGoldMinePrefabEntity,
            BuildingType.OilMine => entityReferenecs.buildingVisualOilMinePrefabEntity,
            _ => entityReferenecs.buildingVisualTowerPrefabEntity,
        };
    }
}
