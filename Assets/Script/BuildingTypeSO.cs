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
    }
    public BuildingType buildingType;
    public GameObject prefab;
    public float buildingDistanceMin;
    public bool showInBuildingPlacementUI;
    public Sprite icon;
    public Transform buildGhost;
    public Entity GetPrefabEntity(EntityReferenecs entityReferenecs)
    {
        return buildingType switch
        {
            BuildingType.Tower => entityReferenecs.buildingTowerPrefabEntity,
            BuildingType.Barracks => entityReferenecs.buildingbarracksPrefabEntity,
            _ => entityReferenecs.buildingTowerPrefabEntity,
        };
    }
}
