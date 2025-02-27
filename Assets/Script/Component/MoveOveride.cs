using Unity.Entities;
using Unity.Mathematics;

public struct MoveOveride : IComponentData, IEnableableComponent
{
    public float3 targetPosition;
}
