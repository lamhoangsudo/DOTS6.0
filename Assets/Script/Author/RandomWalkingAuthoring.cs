using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RandomWalkingAuthoring : MonoBehaviour
{
    public class RandomWalkingAuthoringBaker : Baker<RandomWalkingAuthoring>
    {
        public override void Bake(RandomWalkingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomWalking
            {
                random = new Unity.Mathematics.Random((uint)entity.Index),
            });
        }
    }
}


