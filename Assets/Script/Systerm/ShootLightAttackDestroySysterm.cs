using Unity.Burst;
using Unity.Entities;

partial struct ShootLightAttackDestroySysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach((RefRW<ShootLightAttack> shootLightAttack, Entity shootLightAttackEntity) in SystemAPI.Query<RefRW<ShootLightAttack>>().WithEntityAccess())
        {
            shootLightAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(shootLightAttack.ValueRW.timer <= 0)
            {
                entityCommandBuffer.DestroyEntity(shootLightAttackEntity);
            }
        }
    }
}
