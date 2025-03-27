using Unity.Entities;
using UnityEngine;

public class ActiveAnimationAuthoring : MonoBehaviour
{
    public class ActiveAnimationAuthoringBaker : Baker<ActiveAnimationAuthoring>
    {
        public override void Bake(ActiveAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ActiveAnimation
            {

            });
        }
    }
}


