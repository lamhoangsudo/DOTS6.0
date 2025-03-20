using Unity.Entities;
using UnityEngine;

public class LostTargetAuthoring : MonoBehaviour
{
    [SerializeField] private float lostTargetDistance;
    public class LostTargetAuthoringBaker : Baker<LostTargetAuthoring>
    {
        public override void Bake(LostTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new LostTarget
            {
                lostTargetDistance = authoring.lostTargetDistance,
            });
        }
    }
}


