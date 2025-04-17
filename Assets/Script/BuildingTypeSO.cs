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
}
