using Unity.Entities;
using UnityEngine;

class TargetAuthoring : MonoBehaviour
{
    
}

class TargetAuthoringBaker : Baker<TargetAuthoring>
{
    public override void Bake(TargetAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Target());
    }
}
