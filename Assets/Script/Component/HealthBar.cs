using Unity.Entities;

public struct HealthBar : IComponentData
{
    public Entity barVisual, healthEntity;
}
