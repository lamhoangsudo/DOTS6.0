using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct ZombieSpawnSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntityReferenecs>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityReferenecs entityReferenecs = SystemAPI.GetSingleton<EntityReferenecs>();
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> distanceHitList = new(Allocator.Temp);
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
            distanceHitList.Clear();
            CollisionFilter collisionFilter = new CollisionFilter
            {
                //...11111111
                //belong to all layers
                BelongsTo = ~0u,
                //...00000001
                //...00001000
                //only affects layer 6
                CollidesWith = 1u << GameAssets.UNIT_LAYER,
                GroupIndex = 0,
            };
            if(collisionWorld.OverlapSphere(localTranform.ValueRO.Position, zombieSpawn.ValueRO.nearbyZombieDistance, ref distanceHitList, collisionFilter))
            {
                //zombie spawn area is occupied
                int count = 0;
                foreach (DistanceHit distanceHit in distanceHitList)
                {
                    if (!SystemAPI.HasComponent<Zombie>(distanceHit.Entity) || !SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Unit>(distanceHit.Entity)) continue;
                        count++;
                        
                }
                if (count >= zombieSpawn.ValueRO.nearbyZombieCountMax) continue;
            }
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
