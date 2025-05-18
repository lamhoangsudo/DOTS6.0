using Unity.Entities;
using UnityEngine;

public class MainMenuSceneAuthoring : MonoBehaviour
{
    public class MainMenuSceneAuthoringBaker : Baker<MainMenuSceneAuthoring>
    {
        public override void Bake(MainMenuSceneAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MainMenuSceneTag());
        }
    }
}
public struct MainMenuSceneTag : IComponentData
{
    // This is a tag component, so it doesn't need any fields.
}

