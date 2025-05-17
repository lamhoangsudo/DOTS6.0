using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BuildingContructionAuthoring : MonoBehaviour
{
    public class BuildingContructionAuthoringBaker : Baker<BuildingContructionAuthoring>
    {
        public override void Bake(BuildingContructionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingContruction());
        }
    }
}
public struct BuildingContruction : IComponentData
{
    public float progress;
    public float maxProgress;
    public BuildingTypeSO.BuildingType buildingType;
    public Entity finalPrefabEntity;
    public Entity visualEntity;
    public float3 startPosition;
    public float3 endPosition;
}


