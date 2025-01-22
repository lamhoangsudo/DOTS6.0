using Unity.Burst;
using Unity.Entities;
using Unity.VisualScripting;

partial struct ShootAttackSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<ShootAttack> shootAttack, RefRO<Target> target) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>>())
        {
            if(target.ValueRO.targetEntity == Entity.Null) continue;
            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(shootAttack.ValueRW.timer > 0) continue;
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;
            RefRW<Health> healthTarget = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
            if (healthTarget.ValueRO.health > 0f)
            {
                healthTarget.ValueRW.health -= 1;
            }
            else
            {

            }
        }
    }
}
