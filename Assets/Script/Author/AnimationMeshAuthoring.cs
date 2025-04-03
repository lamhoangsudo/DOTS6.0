using Unity.Entities;
using UnityEngine;

public class AnimationMeshAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject mesh;
    public class AnimationMeshAuthoringBaker : Baker<AnimationMeshAuthoring>
    {
        public override void Bake(AnimationMeshAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AnimationMesh
            {
                meshEntity = GetEntity(authoring.mesh, TransformUsageFlags.Dynamic),
            });
        }
    }
}


