using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitTypeListSO", menuName = "Scriptable Objects/UnitTypeListSO")]
public class UnitTypeListSO : ScriptableObject
{
    public List<UnitTypeSO> unitTypeList;
    public UnitTypeSO GetUnitTypeSO(UnitTypeSO.UnitType unitType)
    {
        foreach (UnitTypeSO unitTypeSO in unitTypeList)
        {
            if (unitTypeSO.unitType == unitType) return unitTypeSO;
        }
        return null;
    }
}
