using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;

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
            RefRW<Target> target,
            RefRO<TargetOveride> targetOveride) 
            in SystemAPI.Query<
                RefRO<LocalTransform>, 
                RefRW<FindTarget>, 
                RefRW<Target>,
                RefRO<TargetOveride>
                >())
        {
            if (targetOveride.ValueRO.targetEntity != Entity.Null)
            {
                //has target point
                target.ValueRW.targetEntity = targetOveride.ValueRO.targetEntity;
                continue;
            }
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
            Entity closestTargetEnity = Entity.Null;
            float closestDistanceEnity = 0f;
            float closestDistanceOffset = 2f;

            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.findingRange, ref distanceHitList, collisionFilter))
            {
                foreach(DistanceHit distanceHit in distanceHitList)
                {
                    //get unit target component to know which faction are
                    if (!SystemAPI.HasComponent<LocalTransform>(distanceHit.Entity) || !SystemAPI.Exists(distanceHit.Entity)) continue;
                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                    if(findTarget.ValueRO.targetFaction == targetUnit.faction)
                    {
                        //valid target
                        if (closestTargetEnity == Entity.Null)
                        {
                            closestTargetEnity = distanceHit.Entity;
                            closestDistanceEnity = distanceHit.Distance;
                        }
                        else
                        {
                            if(distanceHit.Distance + closestDistanceOffset < closestDistanceEnity)
                            {
                                closestTargetEnity = distanceHit.Entity;
                                closestDistanceEnity = distanceHit.Distance;
                            }
                        }
                        target.ValueRW.targetEntity = closestTargetEnity;
                        break;
                    }
                }
            }
        }
    }
}
