using Unity.Entities;
using UnityEngine;

public class UnitAnimationAuthoring : MonoBehaviour
{
    [SerializeField] private AnimationDataSO.AnimationType soldierWalk;
    [SerializeField] private AnimationDataSO.AnimationType soldierIdel;
    [SerializeField] private AnimationDataSO.AnimationType soldierAim;
    [SerializeField] private AnimationDataSO.AnimationType soldierShoot;
    [SerializeField] private AnimationDataSO.AnimationType meleeAttack;
    public class UnitAnimationAuthoringBaker : Baker<UnitAnimationAuthoring>
    {
        public override void Bake(UnitAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitAnimation
            {
                Walk = authoring.soldierWalk,
                Idel = authoring.soldierIdel,
                Aim = authoring.soldierAim,
                Shoot = authoring.soldierShoot,
                MeleeAttack = authoring.meleeAttack,
            });
        }
    }
}


