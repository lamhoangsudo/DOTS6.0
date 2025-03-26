using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDataSO", menuName = "Scriptable Objects/AnimationDataSO")]
public class AnimationDataSO : ScriptableObject
{
    public Mesh[] frames;
    public float frameTimerMax;
}
