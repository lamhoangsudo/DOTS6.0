using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;
    public const int UNIT_LAYER = 6;
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
}
