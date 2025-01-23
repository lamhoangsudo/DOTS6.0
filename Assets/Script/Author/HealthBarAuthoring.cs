using Unity.Entities;
using UnityEngine;

public class HealthBarAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject barVisual, healthEntity;
    public class HeathBarAuthoringBaker : Baker<HealthBarAuthoring>
    {
        public override void Bake(HealthBarAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthBar
            {
                //use TransformUsageFlags.Dynamic entity can modify tranform file
                //however for scale the change applies globally to 3 axes x, y, z
                //to be able to change random scale axis use TransformUsageFlags.NonUniformScale
                //the purpose is to be able to have 4x4 matrix in PostTransformMatrix
                barVisual = GetEntity(authoring.barVisual, TransformUsageFlags.NonUniformScale),
                healthEntity = GetEntity(authoring.healthEntity, TransformUsageFlags.Dynamic),
            });
        }
    }
}


