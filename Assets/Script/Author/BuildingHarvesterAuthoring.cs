using Unity.Entities;
using UnityEngine;

public class BuildingHarvesterAuthoring : MonoBehaviour
{
    [SerializeField] private float harvestTimeMax;
    [SerializeField] private ResourceTypeSO.ResourceType resourceType;
    public class BuildingHarvesterAuthoringBaker : Baker<BuildingHarvesterAuthoring>
    {
        public override void Bake(BuildingHarvesterAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingHarvester
            {
                harvestTimeMax = authoring.harvestTimeMax,
                resourceType = authoring.resourceType
            });
        }
    }
}
public struct BuildingHarvester : IComponentData
{
    public float harvestTime;
    public float harvestTimeMax;
    public ResourceTypeSO.ResourceType resourceType;
}


