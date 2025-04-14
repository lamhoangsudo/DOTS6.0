using Unity.Entities;

public struct AnimationDataHolderObject : IComponentData
{
    public UnityObjectRef<AnimationDataListSO> animationDataListSO;
}
