using Unity.Entities;

public struct Select : IComponentData, IEnableableComponent
{
    public Entity visualEntity;
    public float scale;
}
