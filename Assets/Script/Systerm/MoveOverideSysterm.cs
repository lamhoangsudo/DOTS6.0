using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverideSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRO<MoveOveride> moveOveride,
            EnabledRefRW<MoveOveride> moveOverideEnabled,
            RefRW<UnitMover> unitMover)
            in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<MoveOveride>,
                EnabledRefRW<MoveOveride>,
                RefRW<UnitMover>>())
        {
            if(math.distance(localTransform.ValueRO.Position, moveOveride.ValueRO.targetPosition) > UnitMoveSysterm.REACH_TARGET_DISTANCE_SQ)
            {
                unitMover.ValueRW.movePosition = moveOveride.ValueRO.targetPosition;
            }
            else
            {
                moveOverideEnabled.ValueRW = false;
            }
        }
    }
}
