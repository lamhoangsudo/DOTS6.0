using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoveSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((RefRO<Bullet> bullet, 
            RefRO<Target> target, 
            RefRW<LocalTransform> localTransform,
            Entity entity) 
            in  SystemAPI.Query<RefRO<Bullet>, RefRO<Target>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);
            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;
            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
            if(distanceAfterSq >= distanceBeforeSq)
            {
                //over shoot
                localTransform.ValueRW.Position = targetLocalTransform.Position;
            }
            if (math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) <= 0.2f)
            {
                //close enough deal damage
                RefRW<Health> healthTarget = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                healthTarget.ValueRW.health -= bullet.ValueRO.damage;
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
