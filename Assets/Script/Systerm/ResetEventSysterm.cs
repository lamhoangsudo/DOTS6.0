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
        foreach(RefRW<Select> select in SystemAPI.Query<RefRW<Select>>().WithPresent<Select>())
        {
            select.ValueRW.OnDeSelect = false;
            select.ValueRW.OnSelect = false;
        }
        foreach (RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.OnValueHealthChange = false;
        }
        foreach (RefRW<ShootAttack> shootAttack in SystemAPI.Query<RefRW<ShootAttack>>())
        {
            shootAttack.ValueRW.OnShoot.trigger = false;
        }
    }
}
