using Unity.Entities;
using UnityEngine;

public class ShootVictimAuthoring : MonoBehaviour
{
    [SerializeField] private Transform hitPosition;
    public class ShootVictimAuthoringBaker : Baker<ShootVictimAuthoring>
    {
        
        public override void Bake(ShootVictimAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootVictim
            {
                hitPosition = authoring.hitPosition.localPosition,
            });
        }
    }
}


