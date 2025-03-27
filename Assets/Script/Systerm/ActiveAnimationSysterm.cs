using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

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
            if(!activeAnimation.ValueRO.aimationDataBlobAssetRef.IsCreated)
            {
                activeAnimation.ValueRW.aimationDataBlobAssetRef = animationDataHolder.soliderIdle;
            }
            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
            if(activeAnimation.ValueRO.frameTimer >= activeAnimation.ValueRO.aimationDataBlobAssetRef.Value.frameTimeMax)
            {
                activeAnimation.ValueRW.frameTimer -= activeAnimation.ValueRO.aimationDataBlobAssetRef.Value.frameTimeMax;
                activeAnimation.ValueRW.frame = (activeAnimation.ValueRO.frame + 1) % activeAnimation.ValueRO.aimationDataBlobAssetRef.Value.frameMax;
                materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.aimationDataBlobAssetRef.Value.batchMeshIDBlobArray[activeAnimation.ValueRO.frame];
            }
        }
    }
}
