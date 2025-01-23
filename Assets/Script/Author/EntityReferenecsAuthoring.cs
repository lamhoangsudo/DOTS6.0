using Unity.Entities;
using UnityEngine;

public class EntityReferenecsAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    public class EntityReferenecsAuthoringBaker : Baker<EntityReferenecsAuthoring>
    {
        public override void Bake(EntityReferenecsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityReferenecs
            {
                bulletEntity = GetEntity(authoring.bullet, TransformUsageFlags.Dynamic),
            });
        }
    }
}


