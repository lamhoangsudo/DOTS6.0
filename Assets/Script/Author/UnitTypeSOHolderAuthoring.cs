using Unity.Entities;
using UnityEngine;

public class UnitTypeSOHolderAuthoring : MonoBehaviour
{
    public UnitTypeSO.UnitType unitTypeSO;
    public class UnitTypeAuthoringBaker : Baker<UnitTypeSOHolderAuthoring>
    {
        public override void Bake(UnitTypeSOHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitTypeSOHolder
            {
                unitTypeSO = authoring.unitTypeSO
            });
        }
    }
}
public struct UnitTypeSOHolder : IComponentData
{
    public UnitTypeSO.UnitType unitTypeSO;
}


