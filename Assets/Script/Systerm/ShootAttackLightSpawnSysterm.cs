using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct ShootAttackLightSpawnSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityReferenecs entityReferenecs = SystemAPI.GetSingleton<EntityReferenecs>();
        foreach (RefRO<ShootAttack> shootAttack in SystemAPI.Query<RefRO<ShootAttack>>())
        {
            if(shootAttack.ValueRO.OnShoot.trigger)
            {
                Entity shootAttackLightEntity = state.EntityManager.Instantiate(entityReferenecs.shootAttackLight);
                SystemAPI.SetComponent(shootAttackLightEntity, LocalTransform.FromPosition(shootAttack.ValueRO.OnShoot.position));
            }
        }
    }
}
