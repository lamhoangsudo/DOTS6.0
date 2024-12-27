using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct UnitMoveSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //foreach((
        //    RefRW<LocalTransform> localTransform,
        //    RefRO<UnitMover> unitMover) 
        //    in SystemAPI.Query<
        //        RefRW<LocalTransform>, 
        //        RefRO<UnitMover>>())
        //{
        //    float3 targetPosition = localTransform.ValueRO.Position + new float3(1, 0, 0);
        //    float3 dir = targetPosition - localTransform.ValueRO.Position;
        //    dir = math.normalize(dir);
        //    localTransform.ValueRW.Position += dir * unitMover.ValueRO.unitMover * SystemAPI.Time.DeltaTime;
        //    localTransform.ValueRW.Rotation = quaternion.LookRotation(dir, math.up());
        //}
        UnitMoverJob unitMoverJob = new()
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        unitMoverJob.ScheduleParallel();
        //foreach ((
        //    RefRW<PhysicsVelocity> velocity,
        //    RefRW<LocalTransform> localTransform,
        //    RefRO<UnitMover> unitMover)
        //    in SystemAPI.Query<
        //        RefRW<PhysicsVelocity>,
        //        RefRW<LocalTransform>,
        //        RefRO<UnitMover>>())
        //{
        //    float3 dir = unitMover.ValueRO.movePosition - localTransform.ValueRO.Position;
        //    dir = math.normalize(dir);
        //    localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRW.Rotation, quaternion.LookRotation(dir, math.up()), SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
        //    velocity.ValueRW.Linear = dir * unitMover.ValueRO.moveSpeed;
        //    velocity.ValueRW.Angular = float3.zero;
        //}
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}

