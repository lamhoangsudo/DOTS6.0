using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    public class BulletAuthoringBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet
            {
                damage = authoring.damage,
                speed = authoring.speed,
            });
        }
    }
}


