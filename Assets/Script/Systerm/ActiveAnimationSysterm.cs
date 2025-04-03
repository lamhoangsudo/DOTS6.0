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
        foreach ((RefRW<ActiveAnimation> activeAnimation, RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
        {
            ref AnimationData animationData = ref animationDataHolder.animationDataBlobArray.Value[(int)activeAnimation.ValueRO.activeAnimationType];
            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
            if(activeAnimation.ValueRO.frameTimer >= animationData.frameTimeMax)
            {
                activeAnimation.ValueRW.frameTimer -= animationData.frameTimeMax;
                activeAnimation.ValueRW.frame = (activeAnimation.ValueRO.frame + 1) % animationData.frameMax;
                materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIDBlobArray[activeAnimation.ValueRO.frame];
            }
        }
    }
}
