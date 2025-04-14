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
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        jobHandles = new NativeArray<JobHandle>(4, Allocator.Persistent);
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        jobHandles[0] = new ResetSelectEventJob().ScheduleParallel(state.Dependency);
        jobHandles[1] = new ResetHealthEventJob().ScheduleParallel(state.Dependency);
        jobHandles[2] = new ResetShootAttackEventJob().ScheduleParallel(state.Dependency);
        jobHandles[3] = new ResetMeleeAttackEventJob().ScheduleParallel(state.Dependency);
        state.Dependency = JobHandle.CombineDependencies(jobHandles);
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