using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MeleeAttackSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<RaycastHit> raycastHitList = new(Allocator.Temp);
        foreach (
            (RefRW<MeleeAttack> meleeAttack,
            RefRO<Target> target,
            RefRO<LocalTransform> localTransform,
            RefRW<TargetPositionPathQueued> targetPositionPathQueued,
            EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled)
            in
            SystemAPI.Query<
                RefRW<MeleeAttack>,
                RefRO<Target>,
                RefRO<LocalTransform>,
                RefRW<TargetPositionPathQueued>,
                EnabledRefRW<TargetPositionPathQueued>
                >().WithDisabled<MoveOveride>().WithPresent<TargetPositionPathQueued>())
        {
            if (target.ValueRO.targetEntity == Entity.Null) continue;
            Entity targetEntity = target.ValueRO.targetEntity;
            RefRO<LocalTransform> targetEntityLocalTransform = SystemAPI.GetComponentRO<LocalTransform>(targetEntity);
            float meleeAttackDistance = 2f;
            bool isCloseTargetEnough = math.distancesq(localTransform.ValueRO.Position, targetEntityLocalTransform.ValueRO.Position) <= meleeAttackDistance;
            bool isTouchingTarget = false;
            if (!isCloseTargetEnough)
            {
                RaycastInput raycastInput = new()
                {
                    Start = localTransform.ValueRO.Position,
                    End = localTransform.ValueRO.Position + 
                    (math.normalize(targetEntityLocalTransform.ValueRO.Position - localTransform.ValueRO.Position)) 
                    * meleeAttack.ValueRO.colliderSize
                    + 0.4f,
                    Filter = CollisionFilter.Default,
                };
                raycastHitList.Clear();
                collisionWorld.CastRay(raycastInput, ref raycastHitList);
                foreach (var hit in raycastHitList)
                {
                    if(hit.Entity == targetEntity)
                    {
                        //close enough to attack target
                        isTouchingTarget = true;
                        break;
                    }
                }
                if (!isTouchingTarget)
                {
                    //target is too far
                    targetPositionPathQueued.ValueRW.targetPosition = targetEntityLocalTransform.ValueRO.Position;
                }
            }
            if(isTouchingTarget || isCloseTargetEnough)
            {
                //target is close enough
                targetPositionPathQueued.ValueRW.targetPosition = localTransform.ValueRO.Position;
                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (meleeAttack.ValueRW.timer > 0) continue;
                //attack
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(targetEntity);
                targetHealth.ValueRW.health -= meleeAttack.ValueRO.attackDamage;
                targetHealth.ValueRW.OnValueHealthChange = true;
                meleeAttack.ValueRW.timer = meleeAttack.ValueRO.attackTimerMax;
                meleeAttack.ValueRW.onAttack = true;
            }
        }
    }
}
