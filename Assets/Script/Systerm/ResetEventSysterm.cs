using Unity.Burst;
using Unity.Entities;
//this systerm will run the end of frame
//this code set locate time when systerm run
[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new ResetSelectEventJob().ScheduleParallel();
        new ResetHealthEventJob().ScheduleParallel();
        new ResetShootAttackEventJob().ScheduleParallel();
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