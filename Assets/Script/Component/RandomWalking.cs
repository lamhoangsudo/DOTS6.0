using Unity.Entities;
using Unity.Mathematics;

public struct RandomWalking : IComponentData
{
    public float3 targetPosition;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
    public Random random;
}
