using Unity.Entities;
using UnityEngine.Rendering;

public struct AnimationDataHolder : IComponentData
{
    public BlobAssetReference<AnimationData> soliderIdle;
}
public struct AnimationData
{
    public float frameTimeMax;
    public int frameMax;
    public BlobArray<BatchMeshID> batchMeshIDBlobArray;
}
