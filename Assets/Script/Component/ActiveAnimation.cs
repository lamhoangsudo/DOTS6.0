using Unity.Entities;
using UnityEngine.Rendering;

public struct ActiveAnimation : IComponentData
{
    public int frame;
    public float frameTimer;
    public AnimationDataSO.AnimationType activeAnimationType;
    public AnimationDataSO.AnimationType nextAnimationType;
}
