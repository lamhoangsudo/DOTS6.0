using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupUnitMoveDefaultPositionSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach (
            (RefRO<LocalTransform> localTransform, 
            RefRW<UnitMover> unitMover, 
            RefRO<SetupUnitMoveDefaultPosition> setupUnitMoveDefaultPosition,
            Entity entity) 
            in 
            SystemAPI.Query<RefRO<LocalTransform>, RefRW<UnitMover>, RefRO<SetupUnitMoveDefaultPosition>>().WithEntityAccess())
        {
            unitMover.ValueRW.movePosition = localTransform.ValueRO.Position;
            entityCommandBuffer.SetComponentEnabled<SetupUnitMoveDefaultPosition>(entity, false);
            //entityCommandBuffer.RemoveComponent<SetupUnitMoveDefaultPosition>(entity);
        }
    }
}
