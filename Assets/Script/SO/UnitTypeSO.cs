using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitTypeSO", menuName = "Scriptable Objects/UnitTypeSO")]
public class UnitTypeSO : ScriptableObject
{
    public enum UnitType
    {
        None = 0,
        Soldier = 1,
        Scout = 2,
        Zombie = 3,
    }
    public UnitType unitType;
    public float progress;
    public Sprite sprite;
    public ResourceAmount[] resourceAmounts;
    public GameObject ragdollPrefab;

    public Entity GetPrefabEntity(EntityReferenecs entityReferenecs)
    {
        return unitType switch
        {
            UnitType.Scout => entityReferenecs.scout,
            UnitType.Zombie => entityReferenecs.zombie,
            _ => entityReferenecs.soldier,
        };
    }
}
