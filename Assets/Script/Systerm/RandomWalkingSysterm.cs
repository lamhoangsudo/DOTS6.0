using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<RandomWalking> randomWalking,
            RefRW<TargetPositionPathQueued> targetPositionPathQueued,
            EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled,
            RefRO<LocalTransform> localTransform)
            in
            SystemAPI.Query<
                RefRW<RandomWalking>,
            RefRW<TargetPositionPathQueued>,
            EnabledRefRW<TargetPositionPathQueued>,
            RefRO<LocalTransform>>().WithPresent<TargetPositionPathQueued>())
        {
            if (math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition) < UnitMoveSysterm.REACH_TARGET_DISTANCE_SQ)
            {
                //reach target position
                //random in dots is not a class but a struct
                //meaning it is a value type not a reference type
                //when there is a change, this change exists in a copy not the original
                Random random = randomWalking.ValueRO.random;
                float3 randomDirection = new(random.NextFloat(-1f, 1f), 0, random.NextFloat(-1f, 1f));
                randomDirection = math.normalize(randomDirection);
                randomWalking.ValueRW.targetPosition =
                    randomWalking.ValueRO.originPosition +
                    randomDirection *
                    random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);
                //Because it is a value type, it is necessary to save changes from the copy to the original 
                randomWalking.ValueRW.random = random;
            }
            else
            {
                //move closer
                targetPositionPathQueued.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
                targetPositionPathQueuedEnabled.ValueRW = true;
            }
        }
    }
}
