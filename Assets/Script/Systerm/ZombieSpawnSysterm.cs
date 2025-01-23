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
            if(zombieSpawn.ValueRO.timer > 0)
            {
                continue;
            }
            zombieSpawn.ValueRW.timer = zombieSpawn.ValueRO.timerMax;
            Entity zombieEntity = state.EntityManager.Instantiate(entityReferenecs.zombie);
            RefRW<LocalTransform> zombieLocalTranform = SystemAPI.GetComponentRW<LocalTransform > (zombieEntity);
            zombieLocalTranform.ValueRW.Position = localTranform.ValueRO.Position;
        }
    }
}
