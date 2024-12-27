using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMoverAuthor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_RotationSpeed;
    private float3 m_MovePosition;
    public class Baker : Baker<UnitMoverAuthor>
    {
        public override void Bake(UnitMoverAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMover
            {
                moveSpeed = authoring.m_Speed,
                rotationSpeed = authoring.m_RotationSpeed,
                movePosition = authoring.m_MovePosition,
            });
        }
    }
}
