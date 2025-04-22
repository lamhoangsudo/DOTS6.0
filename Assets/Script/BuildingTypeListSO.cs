using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypeListSO", menuName = "Scriptable Objects/BuildingTypeListSO")]
public class BuildingTypeListSO : ScriptableObject
{
    public List<BuildingTypeSO> buildingTypeList;
    public BuildingTypeSO defaultBuildingTypeSO;
    public BuildingTypeSO GetBuildingTypeSO(BuildingTypeSO.BuildingType buildingType)
    {
        foreach (BuildingTypeSO buildingDataSO in buildingTypeList)
        {
            if (buildingDataSO.buildingType == buildingType) return buildingDataSO;
        }
        return null;
    }
}
