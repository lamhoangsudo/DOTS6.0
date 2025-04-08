using Unity.Entities;
using UnityEngine;

public struct UnitAnimation : IComponentData
{
    public AnimationDataSO.AnimationType Walk;
    public AnimationDataSO.AnimationType Idel;
    public AnimationDataSO.AnimationType Aim;
    public AnimationDataSO.AnimationType Shoot;
    public AnimationDataSO.AnimationType MeleeAttack;
}
