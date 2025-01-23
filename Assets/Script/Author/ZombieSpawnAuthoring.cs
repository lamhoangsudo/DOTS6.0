using Unity.Entities;
using UnityEngine;

public class ZombieSpawnAuthoring : MonoBehaviour
{
    [SerializeField] private float timerMax;
    public class ZombieSpawnAuthoringBaker : Baker<ZombieSpawnAuthoring>
    {
        public override void Bake(ZombieSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawn
            {
                timerMax = authoring.timerMax,
                timer = authoring.timerMax,
            });
        }
    }
}


