using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    [SerializeField] private FactionType faction;
    public class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Unit>(entity, new Unit
            {
            });
        }
    }
}
