using Unity.Burst;
using Unity.Entities;
partial struct AnimationStateSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<AnimationMesh> animationMesh, RefRO<UnitMover> unitMover, RefRO<UnitAnimation> unitAnimation) in SystemAPI.Query<RefRW<AnimationMesh>, RefRO<UnitMover>, RefRO<UnitAnimation>>())
        {
            RefRW<ActiveAnimation>  activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animationMesh.ValueRO.meshEntity);
            if (unitMover.ValueRO.isMoving)
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.soldierWalk;
            }
            else
            {
                activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.soldierIdel;
            }
        }
    }
}
