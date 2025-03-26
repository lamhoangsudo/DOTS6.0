using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class ActiveAnimationAuthoring : MonoBehaviour
{
    [SerializeField] private AnimationDataSO SolidierIdle;
    public class ActiveAnimationAuthoringBaker : Baker<ActiveAnimationAuthoring>
    {
        public override void Bake(ActiveAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            EntitiesGraphicsSystem entityGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
            AddComponent(entity, new ActiveAnimation
            {
                frame0 = entityGraphicsSystem.RegisterMesh(authoring.SolidierIdle.frames[0]),
                frame1 = entityGraphicsSystem.RegisterMesh(authoring.SolidierIdle.frames[1]),
                frameMax = authoring.SolidierIdle.frames.Length,
                frameTimerMax = authoring.SolidierIdle.frameTimerMax
            });
        }
    }
}


