using Unity.Entities;
using UnityEngine;

public class GameSceneTagAuthoring : MonoBehaviour
{
    public class GameSceneTagAuthoringBaker : Baker<GameSceneTagAuthoring>
    {
        public override void Bake(GameSceneTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GameSceneTag());
        }
    }
}
public struct GameSceneTag : IComponentData
{
    // This is a tag component, so it doesn't need any fields.
}
