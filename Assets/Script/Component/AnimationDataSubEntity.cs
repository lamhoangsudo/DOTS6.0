using Unity.Entities;

public struct AnimationDataSubEntity : IComponentData
{
    public AnimationDataSO.AnimationType animationType;
    public int frame;
}
