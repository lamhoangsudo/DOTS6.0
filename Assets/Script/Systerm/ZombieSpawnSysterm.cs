using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityReferenecs entityReferenecs = SystemAPI.GetSingleton<EntityReferenecs>();
        foreach ((
            RefRO<LocalTransform> localTranform,
            RefRW<ZombieSpawn> zombieSpawn)
            in
            SystemAPI.Query<RefRO<LocalTransform>,
            RefRW<ZombieSpawn>>())
        {
            zombieSpawn.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (zombieSpawn.ValueRO.timer > 0)
            {
                continue;
            }
            zombieSpawn.ValueRW.timer = zombieSpawn.ValueRO.timerMax;
            Entity zombieEntity = state.EntityManager.Instantiate(entityReferenecs.zombie);
            SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTranform.ValueRO.Position));
            SystemAPI.SetComponent(zombieEntity, new RandomWalking
            {
                distanceMax = zombieSpawn.ValueRO.zombieRandomWalkingDistanceMax,
                distanceMin = zombieSpawn.ValueRO.zombieRandomWalkingDistanceMin,
                targetPosition = localTranform.ValueRO.Position,
                originPosition = localTranform.ValueRO.Position,
                random = new Unity.Mathematics.Random((uint)zombieEntity.Index),
            });
        }
    }
}
