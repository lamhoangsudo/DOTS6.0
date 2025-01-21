using Unity.Entities;
using UnityEngine;

public class ZombieAuthoring : MonoBehaviour
{
    
}

public class ZombieAuthoringBaker : Baker<ZombieAuthoring>
{
    public override void Bake(ZombieAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<Zombie>(entity, new Zombie());
    }
}
