using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    [SerializeField] private float healthMax;
    public class HealthAuthoringBaker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health
            {
                healthMax = authoring.healthMax,
                health = authoring.healthMax,
                OnValueHealthChange = true,
            });
        }
    }
}


