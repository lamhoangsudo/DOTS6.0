using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
[UpdateBefore(typeof(ActiveAnimationSysterm))]
partial struct ChangeAnimationSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
        foreach ((RefRW<ActiveAnimation> activeAnimation, RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
        {
            if(activeAnimation.ValueRO.activeAnimationType != activeAnimation.ValueRO.nextAnimationType)
            {
                //swapping animation
                //reset data
                activeAnimation.ValueRW.frame = 0;
                activeAnimation.ValueRW.frameTimer = 0f;
                activeAnimation.ValueRW.activeAnimationType = activeAnimation.ValueRO.nextAnimationType;
                //change mesh
                //set mesh for first frame
                ref AnimationData animationData = ref animationDataHolder.animationDataBlobArray.Value[(int)activeAnimation.ValueRO.activeAnimationType];
                materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIDBlobArray[0];
            }
        }
    }
}
