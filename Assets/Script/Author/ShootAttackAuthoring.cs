using Unity.Entities;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{
    [SerializeField] private float timerMax;
    [SerializeField] private float damage;
    [SerializeField] private float attackDistance;
    [SerializeField] private Transform bulletSpawnPosition;
    public class ShootAttackAuthoringBaker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAttack
            {
                timerMax = authoring.timerMax,
                damage = authoring.damage,
                attackDistance = authoring.attackDistance,
                bulletSpawnPosition = authoring.bulletSpawnPosition.localPosition,
            });
        }
    }
}


