using Unity.Entities;
using UnityEngine;

public class SetupUnitMoveDefaultPositionAuthoring : MonoBehaviour
{
    public class SetupUnitMoveDefaultPositionBaker : Baker<SetupUnitMoveDefaultPositionAuthoring>
    {
        public override void Bake(SetupUnitMoveDefaultPositionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SetupUnitMoveDefaultPosition());
        }
    }
}


