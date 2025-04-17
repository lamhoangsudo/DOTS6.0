using Unity.Entities;
using UnityEngine;

public class BuildingTypeSOHolderAuthoring : MonoBehaviour
{
    [SerializeField] private BuildingTypeSO.BuildingType buildingTypeSO;
    public class BuildingTypeSOHolderAuthoringBaker : Baker<BuildingTypeSOHolderAuthoring>
    {
        public override void Bake(BuildingTypeSOHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingTypeSOHolder
            {
                buildingTypeSO = authoring.buildingTypeSO,
            });
        }
    }
}
public struct BuildingTypeSOHolder : IComponentData
{
    public BuildingTypeSO.BuildingType buildingTypeSO;
}


