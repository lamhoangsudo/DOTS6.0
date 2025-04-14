using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
[UpdateBefore(typeof(ActiveAnimationSysterm))]
partial struct ChangeAnimationSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AnimationDataHolder>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /*
        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
        foreach ((RefRW<ActiveAnimation> activeAnimation, RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
        {
            if (activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot || activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.ZombieMeleeAttack)
            {
                continue;
            }
            if (activeAnimation.ValueRO.activeAnimationType != activeAnimation.ValueRO.nextAnimationType)
            {
                //swapping animation
                //reset data
                activeAnimation.ValueRW.frame = 0;
                activeAnimation.ValueRW.frameTimer = 0f;
                activeAnimation.ValueRW.activeAnimationType = activeAnimation.ValueRO.nextAnimationType;
                //change mesh
                //set mesh for first frame
                ref AnimationData animationData = ref animationDataHolder.animationDataBlobArray.Value[(int)activeAnimation.ValueRO.activeAnimationType];
                materialMeshInfo.ValueRW.MeshID = animationData.intMeshIDBlobArray[0];
            }
        }
        */
        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();
        ChangeAnimationJob changeAnimationJob = new()
        {
            animationDataHolder = animationDataHolder,
        };
        changeAnimationJob.ScheduleParallel();
    }
}
[BurstCompile]
public partial struct ChangeAnimationJob : IJobEntity
{
    public AnimationDataHolder animationDataHolder;
    public void Execute(
        ref ActiveAnimation activeAnimation,
        ref MaterialMeshInfo materialMeshInfo)
    {
        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot || activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.ZombieMeleeAttack)
        {
            return;
        }
        if (activeAnimation.activeAnimationType != activeAnimation.nextAnimationType)
        {
            //swapping animation
            //reset data
            activeAnimation.frame = 0;
            activeAnimation.frameTimer = 0f;
            activeAnimation.activeAnimationType = activeAnimation.nextAnimationType;
            //change mesh
            //set mesh for first frame
            ref AnimationData animationData = ref animationDataHolder.animationDataBlobArray.Value[(int)activeAnimation.activeAnimationType];
            materialMeshInfo.Mesh = animationData.intMeshIDBlobArray[0];
        }
    }
}
