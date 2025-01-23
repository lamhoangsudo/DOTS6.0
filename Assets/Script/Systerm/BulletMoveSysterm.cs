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
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float3 hitPosition = targetLocalTransform.TransformPoint(SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity).hitPosition);

            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, hitPosition);

            float3 moveDirection = hitPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);
            localTransform.ValueRW.Position += bullet.ValueRO.speed * SystemAPI.Time.DeltaTime * moveDirection;

            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, hitPosition);
            if(distanceAfterSq > distanceBeforeSq)
            {
                //over shoot
                localTransform.ValueRW.Position = hitPosition;
            }
            if (math.distancesq(localTransform.ValueRO.Position, hitPosition) < 0.2f)
            {
                //close enough deal damage
                RefRW<Health> healthTarget = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                healthTarget.ValueRW.health -= bullet.ValueRO.damage;
                healthTarget.ValueRW.OnValueHealthChange = true;
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
