using Unity.Entities;
using UnityEngine;

public struct UnitAnimation : IComponentData
{
    public AnimationDataSO.AnimationType soldierWalk;
    public AnimationDataSO.AnimationType soldierIdel;
}
