using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct HordeSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntityReferenecs>();    
    }
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityReferenecs entityReferenecs = SystemAPI.GetSingleton<EntityReferenecs>();
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((RefRW<LocalTransform> localTransform, RefRW<Horde> horde) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Horde>>())
        {
            horde.ValueRW.startTimer -= SystemAPI.Time.DeltaTime;
            if(horde.ValueRO.startTimer <= 0)
            {
                //start spawn
                if(horde.ValueRO.spawnCount > 0)
                {
                    //still have spawn count
                    horde.ValueRW.spawnTimer -= SystemAPI.Time.DeltaTime;
                    if(horde.ValueRO.spawnTimer <= 0)
                    {
                        //spawn
                        horde.ValueRW.spawnTimer = horde.ValueRO.spawnTimerMax;
                        Entity zombieEntity = entityCommandBuffer.Instantiate(entityReferenecs.zombie);
                        float3 spawnPosition = localTransform.ValueRO.Position;
                        Unity.Mathematics.Random random = horde.ValueRO.random;
                        spawnPosition.x += random.NextFloat(-horde.ValueRO.spawnAreaWidth, horde.ValueRO.spawnAreaWidth);
                        spawnPosition.z += random.NextFloat(-horde.ValueRO.spawnAreaHeight, horde.ValueRO.spawnAreaHeight);
                        horde.ValueRW.random = random;
                        entityCommandBuffer.SetComponent<LocalTransform>(zombieEntity, LocalTransform.FromPosition(spawnPosition));
                        entityCommandBuffer.AddComponent<EnemyAttackHQ>(zombieEntity);
                        horde.ValueRW.spawnCount--;
                    }
                }
            }
        }
    }
}
