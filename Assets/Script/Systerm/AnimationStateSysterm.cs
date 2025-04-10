using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
[UpdateAfter(typeof(ShootAttackSysterm))]
partial struct AnimationStateSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /*
        foreach ((RefRW<AnimationMesh> animationMesh,
            RefRO<UnitMover> unitMover,
            RefRO<UnitAnimation> unitAnimation)
            in
            SystemAPI.Query<RefRW<AnimationMesh>, RefRO<UnitMover>, RefRO<UnitAnimation>>())
        {
            RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animationMesh.ValueRO.meshEntity);
            if (unitMover.ValueRO.isMoving)
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.Walk;
            }
            else
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.Idel;
            }
        }
        foreach ((RefRW<AnimationMesh> animationMesh,
            RefRO<ShootAttack> shootAttack,
            RefRO<UnitAnimation> unitAnimation,
            RefRO<UnitMover> unitMover,
            RefRO<Target> target)
            in
            SystemAPI.Query<RefRW<AnimationMesh>, RefRO<ShootAttack>, RefRO<UnitAnimation>, RefRO<UnitMover>, RefRO<Target>>())
        {
            RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animationMesh.ValueRO.meshEntity);
            if (!unitMover.ValueRO.isMoving && target.ValueRO.targetEntity != Entity.Null)
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.Aim;
            }
            if (shootAttack.ValueRO.OnShoot.trigger)
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.Shoot;
            }
        }
        foreach ((RefRW<AnimationMesh> animationMesh,
            RefRO<MeleeAttack> meleeAttack,
            RefRO<UnitAnimation> unitAnimation)
            in
            SystemAPI.Query<RefRW<AnimationMesh>, RefRO<MeleeAttack>, RefRO<UnitAnimation>>())
        {
            RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animationMesh.ValueRO.meshEntity);
            if (meleeAttack.ValueRO.onAttack)
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.MeleeAttack;
            }
        */
        
        EntityCommandBuffer entityCommandBufferIdleWalkingAnimationStateJob = new(Allocator.TempJob);
        IdleWalkingAnimationStateJob idleWalkingAnimationStateJob = new()
        {
            componentLookupActiveAnimation = SystemAPI.GetComponentLookup<ActiveAnimation>(isReadOnly: false),
            entityCommandBuffer = entityCommandBufferIdleWalkingAnimationStateJob.AsParallelWriter()
        };
        // Schedule the job to update the animation state based on movement
        idleWalkingAnimationStateJob.ScheduleParallel();
        // Complete the all job dependencies
        state.Dependency.Complete();
        entityCommandBufferIdleWalkingAnimationStateJob.Playback(state.EntityManager);
        entityCommandBufferIdleWalkingAnimationStateJob.Dispose();
        EntityCommandBuffer entityCommandBufferAimAnimationStateJob = new(Allocator.TempJob);
        AimAnimationStateJob aimAnimationStateJob = new()
        {
            componentLookupActiveAnimation = SystemAPI.GetComponentLookup<ActiveAnimation>(isReadOnly: false),
            entityCommandBuffer = entityCommandBufferAimAnimationStateJob.AsParallelWriter()
        };
        // Schedule the job to update the animation state based on aiming
        aimAnimationStateJob.ScheduleParallel();
        // Complete the all job dependencies
        state.Dependency.Complete();
        entityCommandBufferAimAnimationStateJob.Playback(state.EntityManager);
        entityCommandBufferAimAnimationStateJob.Dispose();
        EntityCommandBuffer entityCommandBufferShootAnimationStateJob = new(Allocator.TempJob);
        ShootAnimationStateJob shootAnimationStateJob = new()
        {
            componentLookupActiveAnimation = SystemAPI.GetComponentLookup<ActiveAnimation>(isReadOnly: false),
            entityCommandBuffer = entityCommandBufferShootAnimationStateJob.AsParallelWriter()
        };
        // Schedule the job to update the animation state based on shooting
        shootAnimationStateJob.ScheduleParallel();
        // Complete the all job dependencies
        state.Dependency.Complete();
        entityCommandBufferShootAnimationStateJob.Playback(state.EntityManager);
        entityCommandBufferShootAnimationStateJob.Dispose();
        
    }
}
[BurstCompile]
public partial struct IdleWalkingAnimationStateJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<ActiveAnimation> componentLookupActiveAnimation;
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;
    public void Execute(
        [ChunkIndexInQuery] int sortKey,
        in AnimationMesh animationMesh,
        in UnitMover unitMover,
        in UnitAnimation unitAnimation)
    {
        ActiveAnimation activeAnimationWrite = componentLookupActiveAnimation[animationMesh.meshEntity];
        if (unitMover.isMoving)
        {
            activeAnimationWrite.nextAnimationType = unitAnimation.Walk;
        }
        else
        {
            activeAnimationWrite.nextAnimationType = unitAnimation.Idel;
        }
        entityCommandBuffer.SetComponent<ActiveAnimation>(sortKey, animationMesh.meshEntity, activeAnimationWrite);
    }
}
[BurstCompile]
public partial struct AimAnimationStateJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<ActiveAnimation> componentLookupActiveAnimation;
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;
    public void Execute([ChunkIndexInQuery] int sortKey,
        in AnimationMesh animationMesh,
            in ShootAttack shootAttack,
            in UnitAnimation unitAnimation,
            in UnitMover unitMover,
            in Target target)
    {
        ActiveAnimation activeAnimationWrite = componentLookupActiveAnimation[animationMesh.meshEntity];
        if (!unitMover.isMoving && target.targetEntity != Entity.Null)
        {
            activeAnimationWrite.nextAnimationType = unitAnimation.Aim;
        }
        if (shootAttack.OnShoot.trigger)
        {
            activeAnimationWrite.nextAnimationType = unitAnimation.Shoot;
        }
        entityCommandBuffer.SetComponent<ActiveAnimation>(sortKey, animationMesh.meshEntity, activeAnimationWrite);
    }
}
[BurstCompile]
public partial struct ShootAnimationStateJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<ActiveAnimation> componentLookupActiveAnimation;
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;
    public void Execute([ChunkIndexInQuery] int sortKey,
        in AnimationMesh animationMesh,
            in MeleeAttack meleeAttack,
            in UnitAnimation unitAnimation)
    {
        ActiveAnimation activeAnimationWrite = componentLookupActiveAnimation[animationMesh.meshEntity];
        if (meleeAttack.onAttack)
        {
            activeAnimationWrite.nextAnimationType = unitAnimation.MeleeAttack;
        }
        entityCommandBuffer.SetComponent<ActiveAnimation>(sortKey, animationMesh.meshEntity, activeAnimationWrite);
    }
}

