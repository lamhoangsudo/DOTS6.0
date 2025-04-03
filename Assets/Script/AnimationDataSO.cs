using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDataSO", menuName = "Scriptable Objects/AnimationDataSO")]
public class AnimationDataSO : ScriptableObject
{
    public enum AnimationType
    {
        None = 0,
        SoldierIdel = 1,
        SoldierWalk = 2,
        ZombieIdel = 3,
        ZombieWalk = 4,
    }
    public AnimationType animationType;
    public Mesh[] frames;
    public float frameTimerMax;
}
