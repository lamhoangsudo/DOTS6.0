using Unity.Entities;
using UnityEngine;

public class ResourceTypeSOHolderAuthoring : MonoBehaviour
{
    [SerializeField] private ResourceTypeSO resourceTypeSO;
    public class ResourceTypeSOHolderAuthoringBaker : Baker<ResourceTypeSOHolderAuthoring>
    {
        public override void Bake(ResourceTypeSOHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResourceTypeSOHolder
            {
                resourceType = authoring.resourceTypeSO.resourceType
            });
        }
    }
}
public struct ResourceTypeSOHolder : IComponentData
{
    public ResourceTypeSO.ResourceType resourceType;
}

