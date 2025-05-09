using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
partial struct ShootAttackSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntityReferenecs>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityReferenecs entitiesReferences = SystemAPI.GetSingleton<EntityReferenecs>();

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<ShootAttack> shootAttack,
            RefRO<Target> target,
            RefRW<UnitMover> unitMover,
            RefRW<TargetPositionPathQueued> targetPositionPathQueued,
            EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled,
            Entity unitEntity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>,
                RefRW<UnitMover>,
                RefRW<TargetPositionPathQueued>,
                EnabledRefRW<TargetPositionPathQueued>>().WithDisabled<MoveOveride>().WithPresent<TargetPositionPathQueued>().WithEntityAccess())
        {

            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }
            
            RefRO<LocalTransform> targetLocalTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.targetEntity);
            float distance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.ValueRO.Position);
            if (distance > shootAttack.ValueRO.attackDistance)
            {
                targetPositionPathQueued.ValueRW.targetPosition = targetLocalTransform.ValueRO.Position;
                continue;
            }
            else
            {
                targetPositionPathQueued.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }
            float3 dir = targetLocalTransform.ValueRO.Position - localTransform.ValueRO.Position;
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(dir, math.up()), SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
            targetPositionPathQueuedEnabled.ValueRW = true;
            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0f)
            {
                continue;
            }
        }
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<ShootAttack> shootAttack,
            RefRO<Target> target,
            Entity unitEntity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>>().WithEntityAccess())
        {

            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            RefRO<LocalTransform> targetLocalTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.targetEntity);
            float distance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.ValueRO.Position);
            if (distance > shootAttack.ValueRO.attackDistance)
            {
                continue;
            }
            if (SystemAPI.HasComponent<MoveOveride>(unitEntity) &&  SystemAPI.IsComponentEnabled<MoveOveride>(unitEntity))
            {
                continue;
            }
            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0f)
            {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

            if (SystemAPI.HasComponent<TargetOveride>(target.ValueRO.targetEntity))
            {
                RefRW<TargetOveride> enemyTargetOveride = SystemAPI.GetComponentRW<TargetOveride>(target.ValueRO.targetEntity);
                if (enemyTargetOveride.ValueRO.targetEntity == Entity.Null)
                {
                    enemyTargetOveride.ValueRW.targetEntity = unitEntity;
                }
            }

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletEntity);
            float3 spawnBulletPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(spawnBulletPosition));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damage = shootAttack.ValueRO.damage;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;

            shootAttack.ValueRW.OnShoot.trigger = true;
            shootAttack.ValueRW.OnShoot.position = spawnBulletPosition;
        }
    }
}
