using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
//this systerm will run the end of frame
//this code set locate time when systerm run
[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventSysterm : ISystem
{
    private NativeArray<JobHandle> jobHandles;
    private NativeList<Entity> onUnitQueueChangedEntityList;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        jobHandles = new NativeArray<JobHandle>(5, Allocator.Persistent);
        onUnitQueueChangedEntityList = new(Allocator.Persistent);
    }
    public void OnUpdate(ref SystemState state)
    {
        if(SystemAPI.HasSingleton<BuildingHQ>())
        {
            Health health = SystemAPI.GetComponent<Health>(SystemAPI.GetSingletonEntity<BuildingHQ>());
            if(health.OnDead)
            {
                DOTSEventsManager.Instance.TriggerOnHQDead();
            }
        }
        jobHandles[0] = new ResetSelectEventJob().ScheduleParallel(state.Dependency);
        jobHandles[1] = new ResetHealthEventJob().ScheduleParallel(state.Dependency);
        jobHandles[2] = new ResetShootAttackEventJob().ScheduleParallel(state.Dependency);
        jobHandles[3] = new ResetMeleeAttackEventJob().ScheduleParallel(state.Dependency);
        onUnitQueueChangedEntityList.Clear();
        new ResetBuildingBarracksEventJob() 
        {
            onUnitQueueChangedEntityList = onUnitQueueChangedEntityList.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();
        DOTSEventsManager.Instance.TriggerOnBarracksUnitQueueChanged(onUnitQueueChangedEntityList);
        state.Dependency = JobHandle.CombineDependencies(jobHandles);
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        jobHandles.Dispose();
        onUnitQueueChangedEntityList.Dispose();
    }
}
[BurstCompile]
partial struct ResetShootAttackEventJob : IJobEntity
{
    public void Execute(ref ShootAttack shootAttack)
    {
        shootAttack.OnShoot.trigger = false;
    }
}
[BurstCompile]
partial struct ResetHealthEventJob : IJobEntity
{
    public void Execute(ref Health health)
    {
        health.OnValueHealthChange = false;
        health.OnDead = false;
    }
}
[BurstCompile]
[WithPresent(typeof(Select))]
partial struct ResetSelectEventJob : IJobEntity
{
    public void Execute(ref Select select)
    {
        select.OnDeSelect = false;
        select.OnSelect = false;
    }
}
[BurstCompile]
partial struct ResetMeleeAttackEventJob : IJobEntity
{
    public void Execute(ref MeleeAttack meleeAttack)
    {
        meleeAttack.onAttack = false;
    }
}
[BurstCompile]
partial struct ResetBuildingBarracksEventJob : IJobEntity
{
    public NativeList<Entity>.ParallelWriter onUnitQueueChangedEntityList;
    public void Execute(ref BuildingBarracks buildingBarracks, Entity entity)
    {
        if (buildingBarracks.onUnitQueueChanged)
        {
            onUnitQueueChangedEntityList.AddNoResize(entity);
        }
        buildingBarracks.onUnitQueueChanged = false;
    }
}