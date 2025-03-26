using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSysterm : ISystem
{
    private EntityStorageInfoLookup entityStorageInfoLookup;
    private ComponentLookup<LocalTransform> localTransformComponentLookup;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
        localTransformComponentLookup = state.GetComponentLookup<LocalTransform>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        entityStorageInfoLookup.Update(ref state);
        localTransformComponentLookup.Update(ref state);
        ResetTargetJob resetTargetJob = new()
        {
            entityStorageInfoLookup = entityStorageInfoLookup,
            localTransformComponentLookup = localTransformComponentLookup,
        };
        resetTargetJob.ScheduleParallel();
        ResetTargetOverrideJob resetTargetOverideJob = new()
        {
            entityStorageInfoLookup = entityStorageInfoLookup,
            localTransformComponentLookup = localTransformComponentLookup,
        };
        resetTargetOverideJob.ScheduleParallel();
        //foreach(RefRW<Target> targetOveride in SystemAPI.Query<RefRW<Target>>())
        //{
        //    if (targetOveride.ValueRO.targetEntity == Entity.Null) continue;
        //    if(!SystemAPI.Exists(targetOveride.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(targetOveride.ValueRO.targetEntity))
        //    {
        //        targetOveride.ValueRW.targetEntity = Entity.Null;
        //    }
        //}
        //foreach (RefRW<TargetOveride> targetOveride in SystemAPI.Query<RefRW<TargetOveride>>())
        //{
        //    if (targetOveride.ValueRO.targetEntity == Entity.Null) continue;
        //    if (!SystemAPI.Exists(targetOveride.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(targetOveride.ValueRO.targetEntity))
        //    {
        //        targetOveride.ValueRW.targetEntity = Entity.Null;
        //    }
        //}
    }
}
[BurstCompile]
partial struct ResetTargetOverrideJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;

    public void Execute(ref TargetOveride targetOveride)
    {
        if (targetOveride.targetEntity == Entity.Null) return;
        if (!entityStorageInfoLookup.Exists(targetOveride.targetEntity) || !localTransformComponentLookup.HasComponent(targetOveride.targetEntity))
        {
            targetOveride.targetEntity = Entity.Null;
        }
    }
}
[BurstCompile]
partial struct ResetTargetJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;

    public void Execute(ref Target target)
    {
        if (target.targetEntity == Entity.Null) return;
        if (!entityStorageInfoLookup.Exists(target.targetEntity) || !localTransformComponentLookup.HasComponent(target.targetEntity))
        {
            target.targetEntity = Entity.Null;
        }
    }
}
