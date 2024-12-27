using Unity.Entities;
using Unity.Mathematics;

public struct UnitMover : IComponentData
{
    public float moveSpeed;
    public float rotationSpeed;
    public float3 movePosition;
}
