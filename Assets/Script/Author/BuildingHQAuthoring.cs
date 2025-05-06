using Unity.Entities;
using UnityEngine;

public class BuildingHQAuthoring : MonoBehaviour
{
    public class BuildingHQAuthoringBaker : Baker<BuildingHQAuthoring>
    {
        public override void Bake(BuildingHQAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BuildingHQ>(entity);
        }
    }
}
public struct BuildingHQ : IComponentData
{

}


