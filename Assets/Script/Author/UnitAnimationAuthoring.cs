using Unity.Entities;
using UnityEngine;

public class UnitAnimationAuthoring : MonoBehaviour
{
    [SerializeField] private AnimationDataSO.AnimationType soldierWalk;
    [SerializeField] private AnimationDataSO.AnimationType soldierIdel;
    public class UnitAnimationAuthoringBaker : Baker<UnitAnimationAuthoring>
    {
        public override void Bake(UnitAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitAnimation
            {
                soldierWalk = authoring.soldierWalk,
                soldierIdel = authoring.soldierIdel,
            });
        }
    }
}


