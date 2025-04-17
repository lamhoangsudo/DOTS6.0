using Unity.Entities;
using UnityEngine;

public class ZombieSpawnAuthoring : MonoBehaviour
{
    [SerializeField] private float timerMax, RandomWalkingDistanceMax, RandomWalkingDistanceMin, nearbyZombieDistance;
    [SerializeField] private int nearbyZombieCountMax;
    public class ZombieSpawnAuthoringBaker : Baker<ZombieSpawnAuthoring>
    {
        public override void Bake(ZombieSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawn
            {
                timerMax = authoring.timerMax,
                timer = authoring.timerMax,
                zombieRandomWalkingDistanceMax = authoring.RandomWalkingDistanceMax,
                zombieRandomWalkingDistanceMin = authoring.RandomWalkingDistanceMin,
                nearbyZombieCountMax = authoring.nearbyZombieCountMax,
                nearbyZombieDistance = authoring.nearbyZombieDistance,
            });
        }
    }
}


