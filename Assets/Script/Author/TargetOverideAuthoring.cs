using Unity.Entities;
using UnityEngine;

public class TargetOverideAuthoring : MonoBehaviour
{
    public class TargetOverrideAuthoringBaker : Baker<TargetOverideAuthoring>
    {
        public override void Bake(TargetOverideAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TargetOveride());
        }
    }
}


