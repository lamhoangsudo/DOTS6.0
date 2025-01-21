using Unity.Entities;
using UnityEngine;

public class FriendlyAuthoring : MonoBehaviour
{
    
}

public class FriendlyAuthoringBaker : Baker<FriendlyAuthoring>
{
    public override void Bake(FriendlyAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<Friendly>(entity, new Friendly());
    }
}
