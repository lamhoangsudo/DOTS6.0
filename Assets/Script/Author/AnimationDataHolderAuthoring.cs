using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class AnimationDataHolderAuthoring : MonoBehaviour
{
    [SerializeField] private AnimationDataSO soldierIdlel;
    public class AnimationDataHolderAuthoringBaker : Baker<AnimationDataHolderAuthoring>
    {
        public override void Bake(AnimationDataHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AnimationDataHolder animationDataHolder = new();
            EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
            //
            BlobBuilder blobBuilder = new(Allocator.Temp);
            //error work with copy not actually data
            ref AnimationData animationData = ref blobBuilder.ConstructRoot<AnimationData>();
            animationData.frameTimeMax = authoring.soldierIdlel.frameTimerMax;
            animationData.frameMax = authoring.soldierIdlel.frames.Length;
            BlobBuilderArray<BatchMeshID> blobBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationData.batchMeshIDBlobArray, authoring.soldierIdlel.frames.Length);
            for(int i = 0; i < authoring.soldierIdlel.frames.Length; i++)
            {
                BatchMeshID batchMeshID = new();
                batchMeshID = entitiesGraphicsSystem.RegisterMesh(authoring.soldierIdlel.frames[i]);
                blobBuilderArray[i] = batchMeshID;
            }
            //
            animationDataHolder.soliderIdle = blobBuilder.CreateBlobAssetReference<AnimationData>(Allocator.Persistent);
            //error memory leak
            blobBuilder.Dispose();
            //error unallocated memory
            AddBlobAsset(ref animationDataHolder.soliderIdle, out _);
            AddComponent(entity, animationDataHolder);
        }
    }
}


