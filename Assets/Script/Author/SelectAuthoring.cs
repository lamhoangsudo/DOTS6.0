using Unity.Entities;
using UnityEngine;

public class SelectAuthoring : MonoBehaviour
{
    public GameObject selectVisual;
    public float scale;
    public class Baker : Baker<SelectAuthoring>
    {
        public override void Bake(SelectAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Select()
            {
                visualEntity = GetEntity(authoring.selectVisual, TransformUsageFlags.Dynamic),
                scale = authoring.scale,
            });
            SetComponentEnabled<Select>(entity, false);
        }
    }
}

