using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct LostTargetSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<Target> target, RefRO<LocalTransform> localTranform, RefRO<LostTarget> lostTarget) in SystemAPI.Query<RefRW<Target>, RefRO<LocalTransform>, RefRO<LostTarget>>())
        {
            if (target.ValueRO.targetEntity != Entity.Null)
            {
                LocalTransform targetPosition = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                float distance = math.distance(localTranform.ValueRO.Position, targetPosition.Position);
                if (distance >= lostTarget.ValueRO.lostTargetDistance)
                {
                    //target is to far, reset it
                    target.ValueRW.targetEntity = Entity.Null;
                }
            }
        }
    }
}
