using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemyAttackHQSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildingHQ>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity entityHQ = SystemAPI.GetSingletonEntity<BuildingHQ>();
        float3 HQPosition = SystemAPI.GetComponent<LocalTransform>(entityHQ).Position;
        foreach((RefRO<EnemyAttackHQ> enemyAttackHQ, RefRW<UnitMover> unitMover, RefRO<Target> target) in SystemAPI.Query<RefRO<EnemyAttackHQ>, RefRW<UnitMover>, RefRO<Target>>().WithDisabled<MoveOveride>())
        {
            if(target.ValueRO.targetEntity != Entity.Null)
            {
                continue;
            }
            unitMover.ValueRW.movePosition = HQPosition;
        }
    }
}
