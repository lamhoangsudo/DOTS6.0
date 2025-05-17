using Unity.Entities;
using UnityEngine;

public class HordeAuthoring : MonoBehaviour
{
    [SerializeField] private float startTimer;
    [SerializeField] private float spawnTimerMax;
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnAreaWidth;
    [SerializeField] private float spawnAreaHeight;

    [SerializeField] 

    public class HordeAuthoringBaker : Baker<HordeAuthoring>
    {
        public override void Bake(HordeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Horde
            {
                startTimer = authoring.startTimer,
                spawnTimerMax = authoring.spawnTimerMax,
                spawnCount = authoring.spawnCount,
                spawnAreaWidth = authoring.spawnAreaWidth,
                spawnAreaHeight = authoring.spawnAreaHeight,
                random = new((uint)entity.Index),
            });
        }
    }
}
public struct Horde : IComponentData
{
    public float startTimer;
    public float spawnTimer;
    public float spawnTimerMax;
    public int spawnCount;
    public float spawnAreaWidth;
    public float spawnAreaHeight;
    public Unity.Mathematics.Random random;
}

