using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null) continue;
            if(!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
            {
                target.ValueRW.targetEntity = Entity.Null;
            }
        }
        foreach (RefRW<TargetOveride> targetOveride in SystemAPI.Query<RefRW<TargetOveride>>())
        {
            if (targetOveride.ValueRO.targetEntity == Entity.Null) continue;
            if (!SystemAPI.Exists(targetOveride.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(targetOveride.ValueRO.targetEntity))
            {
                targetOveride.ValueRW.targetEntity = Entity.Null;
            }
        }
    }
}
