using Unity.Entities;
using UnityEngine;

public class ActiveAnimationAuthoring : MonoBehaviour
{
    [SerializeField] private AnimationDataSO.AnimationType nextAnimationType;
    public class ActiveAnimationAuthoringBaker : Baker<ActiveAnimationAuthoring>
    {
        public override void Bake(ActiveAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ActiveAnimation
            {
                nextAnimationType = authoring.nextAnimationType,
            });
        }
    }
}


