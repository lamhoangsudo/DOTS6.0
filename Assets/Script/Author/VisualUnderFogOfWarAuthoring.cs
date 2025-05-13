using Unity.Entities;
using UnityEngine;

public class VisualUnderFogOfWarAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject parentEntity;
    [SerializeField] private float sphereCastSize;
    public class VisualUnderFogOfWarAuthoringBaker : Baker<VisualUnderFogOfWarAuthoring>
    {
        public override void Bake(VisualUnderFogOfWarAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VisualUnderFogOfWar
            {
                isVisible = true,
                parentEntity = GetEntity(authoring.parentEntity, TransformUsageFlags.Dynamic),
                sphereCastSize = authoring.sphereCastSize,
            });
        }
    }
}
public struct VisualUnderFogOfWar : IComponentData
{
    public bool isVisible;
    public Entity parentEntity;
    public float sphereCastSize;
}


