using Unity.Entities;
using UnityEngine;

public class EntityReferenecsAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject bullet, zombie, shootLight;
    public class EntityReferenecsAuthoringBaker : Baker<EntityReferenecsAuthoring>
    {
        public override void Bake(EntityReferenecsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityReferenecs
            {
                bulletEntity = GetEntity(authoring.bullet, TransformUsageFlags.Dynamic),
                zombie = GetEntity(authoring.zombie, TransformUsageFlags.Dynamic),
                shootAttackLight = GetEntity(authoring.shootLight, TransformUsageFlags.Dynamic),
            });
        }
    }
}


