using Unity.Entities;
using UnityEngine;

public class MeleeAttackAuthoring : MonoBehaviour
{
    [SerializeField] private float attackDamege;
    [SerializeField] private float attackTimerMax;
    [SerializeField] private float colliderSize;
    public class MeleeAttackAuthoringBaker : Baker<MeleeAttackAuthoring>
    {
        public override void Bake(MeleeAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MeleeAttack
            {
                attackDamage = authoring.attackDamege,
                attackTimerMax = authoring.attackTimerMax,
                colliderSize =authoring.colliderSize,
            });
        }
    }
}


