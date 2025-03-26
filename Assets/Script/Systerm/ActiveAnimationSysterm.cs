using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

partial struct ActiveAnimationSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<ActiveAnimation> activeAnimation, RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
        {
            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
            if(activeAnimation.ValueRO.frameTimer >= activeAnimation.ValueRO.frameTimerMax)
            {
                activeAnimation.ValueRW.frameTimer -= activeAnimation.ValueRO.frameTimerMax;
                activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % activeAnimation.ValueRO.frameMax;
                switch(activeAnimation.ValueRO.frame)
                {
                    case 0:
                        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame0;
                        break;
                    case 1:
                        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame1;
                        break;
                }
            }
        }
    }
}
