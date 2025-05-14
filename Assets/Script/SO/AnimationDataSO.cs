using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDataSO", menuName = "Scriptable Objects/AnimationDataSO")]
public class AnimationDataSO : ScriptableObject
{
    public enum AnimationType
    {
        SoldierNone = 0,
        SoldierIdel = 1,
        SoldierWalk = 2,
        ZombieIdel = 3,
        ZombieWalk = 4,
        SoldierAim = 5,
        SoldierShoot = 6,
        ZombieMeleeAttack = 7,
        ZombieNone = 8,
        ScoutWalk = 9,
        ScoutIdel = 10,
        ScoutAim = 11,
        ScoutShoot = 12,
    }
    public AnimationType animationType;
    public Mesh[] frames;
    public float frameTimerMax;
    public static bool IsAnimationUninterruptible(AnimationType animationType)
    {
        return animationType switch
        {
            AnimationType.SoldierShoot or AnimationType.ScoutShoot or AnimationType.ZombieMeleeAttack => true,
            _ => false,
        };
    }
}
