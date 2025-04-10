using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

partial struct ActiveAnimationSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AnimationDataHolder>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
        ActiveAnimationJob activeAnimationJob = new()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            animationDataHolder = animationDataHolder
        };
        activeAnimationJob.ScheduleParallel();
    }
}
[BurstCompile]
public partial struct ActiveAnimationJob : IJobEntity
{
    public float deltaTime;
    public AnimationDataHolder animationDataHolder;
    public void Execute(
        ref ActiveAnimation activeAnimation,
        ref MaterialMeshInfo materialMeshInfo)
    {
        ref AnimationData animationData = ref animationDataHolder.animationDataBlobArray.Value[(int)activeAnimation.activeAnimationType];
        activeAnimation.frameTimer += deltaTime;
        if (activeAnimation.frameTimer >= animationData.frameTimeMax)
        {
            activeAnimation.frameTimer -= animationData.frameTimeMax;
            activeAnimation.frame = (activeAnimation.frame + 1) % animationData.frameMax;
            materialMeshInfo.MeshID = animationData.batchMeshIDBlobArray[activeAnimation.frame];
            if (activeAnimation.frame == 0)
            {
                if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot) activeAnimation.activeAnimationType = AnimationDataSO.AnimationType.SoldierNone;
                if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.ZombieMeleeAttack) activeAnimation.activeAnimationType = AnimationDataSO.AnimationType.SoldierNone;
            }
        }
    }
}
