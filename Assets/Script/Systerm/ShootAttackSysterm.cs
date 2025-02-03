using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
partial struct ShootAttackSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityReferenecs entitiesReferences = SystemAPI.GetSingleton<EntityReferenecs>();

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<ShootAttack> shootAttack,
            RefRO<Target> target,
            RefRW<UnitMover> unitMover)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>,
                RefRW<UnitMover>>())
        {

            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }
            
            RefRO<LocalTransform> targetLocalTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.targetEntity);
            float distance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.ValueRO.Position);
            if (distance > shootAttack.ValueRO.attackDistance)
            {
                unitMover.ValueRW.movePosition = targetLocalTransform.ValueRO.Position;
                continue;
            }
            else
            {
                unitMover.ValueRW.movePosition = localTransform.ValueRO.Position;
            }
            float3 dir = targetLocalTransform.ValueRO.Position - localTransform.ValueRO.Position;
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(dir, math.up()), SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0f)
            {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

            

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
