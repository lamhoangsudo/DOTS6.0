using Unity.Entities;
using UnityEngine;

public class FactionAuthoring : MonoBehaviour
{
    [SerializeField] private FactionType faction;
    public class FactionAuthoringBaker : Baker<FactionAuthoring>
    {
        public override void Bake(FactionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Faction
            {
                factionType = authoring.faction,
            });
        }
    }
}


