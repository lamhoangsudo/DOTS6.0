using Unity.Entities;
using UnityEngine;

public class ShootLightAttackAuthoring : MonoBehaviour
{
    [SerializeField] private float timer;
    public class ShootLightAttackAuthoringBaker : Baker<ShootLightAttackAuthoring>
    {
        public override void Bake(ShootLightAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ShootLightAttack>(entity, new ShootLightAttack
            {
                timer = authoring.timer,
            });
        }
    }
}


