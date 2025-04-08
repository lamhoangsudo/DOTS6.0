using Unity.Burst;
using Unity.Entities;
[UpdateBefore(typeof(ShootAttack))]
partial struct AnimationStateSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<AnimationMesh> animationMesh, 
            RefRO<UnitMover> unitMover, 
            RefRO<UnitAnimation> unitAnimation) 
            in 
            SystemAPI.Query<RefRW<AnimationMesh>, RefRO<UnitMover>, RefRO<UnitAnimation>>())
        {
            RefRW<ActiveAnimation>  activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animationMesh.ValueRO.meshEntity);
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
            if(!unitMover.ValueRO.isMoving && target.ValueRO.targetEntity != Entity.Null)
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
        }
    }
}
