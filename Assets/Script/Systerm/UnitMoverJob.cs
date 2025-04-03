using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;
    public void Execute
        (
        ref PhysicsVelocity velocity,
        ref LocalTransform localTransform,
        ref UnitMover unitMover
        )
    {
        float3 dir = unitMover.movePosition - localTransform.Position;
        if(math.lengthsq(dir) < UnitMoveSysterm.REACH_TARGET_DISTANCE_SQ)
        {
            unitMover.isMoving = false;
            velocity.Linear = float3.zero;
            velocity.Angular = float3.zero;
            return;
        }
        unitMover.isMoving = true;
        dir = math.normalize(dir);
        localTransform.Rotation = math.slerp(localTransform.Rotation, quaternion.LookRotation(dir, math.up()), deltaTime * unitMover.rotationSpeed);
        velocity.Linear = dir * unitMover.moveSpeed;
        velocity.Angular = float3.zero;
    }
}
