using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> distanceHitList = new(Allocator.Temp);
        foreach((
            RefRO<LocalTransform> localTransform, 
            RefRW<FindTarget> findTarget,
            RefRW<Target> target) 
            in SystemAPI.Query<
                RefRO<LocalTransform>, 
                RefRW<FindTarget>, 
                RefRW<Target>
                >())
        {
            //count down time find target
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(findTarget.ValueRO.timer > 0)
            {
                //timer not elapsed
                continue;
            }
            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;
            distanceHitList.Clear();
            CollisionFilter collisionFilter = new()
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNIT_LAYER,
                GroupIndex = 0,
            };
            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.findingRange, ref distanceHitList, collisionFilter))
            {
                foreach(DistanceHit distanceHit in distanceHitList)
                {
                    //get unit target component to know which faction are
                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                    if(findTarget.ValueRO.targetFaction == targetUnit.faction)
                    {
                        //valid target
                        target.ValueRW.targetEntity = distanceHit.Entity;
                        break;
                    }
                }
            }
        }
    }
}
