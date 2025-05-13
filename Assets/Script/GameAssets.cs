using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;
    public const int UNIT_LAYER = 6, BUILDINGS_LAYER = 7, PATHFINDING_WALL = 8, PATHFINDING_HEAVY = 9, FOG_OF_WAR = 10;
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
    public UnitTypeListSO unitTypeListSO;
    public BuildingTypeListSO buildingTypeListSO;
}
