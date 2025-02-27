using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MoveOverideAuthoring : MonoBehaviour
{
    public class MoveOverideAuthoringBaker : Baker<MoveOverideAuthoring>
    {
        
        public override void Bake(MoveOverideAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveOveride());
            SetComponentEnabled<MoveOveride>(entity, false);
        }
    }
}


