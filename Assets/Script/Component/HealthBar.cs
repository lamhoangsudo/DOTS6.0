using Unity.Entities;
using Unity.Mathematics;

public struct HealthBar : IComponentData
{
    public Entity barVisual, healthEntity;
    public float3 cameraVector;
}
