using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEditor.PackageManager;
using static AnimationDataSO;
using UnityEngine.Rendering;
[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
[UpdateInGroup(typeof(PostBakingSystemGroup))]
partial struct AnimationDataHolderBakingSysterm : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AnimationDataHolderObject>();
    }
    public void OnUpdate(ref SystemState state)
    {
        AnimationDataListSO animationDataListSO = null;
        foreach (RefRO<AnimationDataHolderObject> animationDataHolderObject in SystemAPI.Query<RefRO<AnimationDataHolderObject>>())
        {
            animationDataListSO = animationDataHolderObject.ValueRO.animationDataListSO;
        }
        Dictionary<AnimationDataSO.AnimationType, int[]> blodAssetDataDictionary = new();
        foreach (AnimationDataSO.AnimationType animationType in System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)))
        {
            AnimationDataSO animationDataSO = animationDataListSO.GetAnimationDataSO(animationType);
            blodAssetDataDictionary[animationType] = new int[animationDataSO.frames.Length];
        }
        foreach ((RefRO<AnimationDataSubEntity> animationDataSubEntity, RefRO<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRO<AnimationDataSubEntity>, RefRO<MaterialMeshInfo>>())
        {
            //only return the runtime mesh
            //materialMeshInfo.ValueRO.MeshID
            //return the ID mesh
            //materialMeshInfo.ValueRO.Mesh
            blodAssetDataDictionary[animationDataSubEntity.ValueRO.animationType][animationDataSubEntity.ValueRO.frame] = materialMeshInfo.ValueRO.Mesh;
        }
        foreach (RefRW<AnimationDataHolder> animationDataHolder in SystemAPI.Query<RefRW<AnimationDataHolder>>())
        {
            //
            BlobBuilder blobBuilder = new(Allocator.Temp);
            //error work with copy not actually data
            ref BlobArray<AnimationData> animationDataBlobArray = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();
            BlobBuilderArray<AnimationData> blobBuilderAnimationDataArray = blobBuilder.Allocate<AnimationData>(ref animationDataBlobArray, System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)).Length);
            //
            foreach (AnimationDataSO.AnimationType animationType in System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)))
            {
                AnimationDataSO animationDataSO = animationDataListSO.GetAnimationDataSO(animationType);
                blobBuilderAnimationDataArray[(int)animationType].frameTimeMax = animationDataSO.frameTimerMax;
                blobBuilderAnimationDataArray[(int)animationType].frameMax = animationDataSO.frames.Length;
                BlobBuilderArray<int> blobBuilderMeshArray = blobBuilder.Allocate<int>(ref blobBuilderAnimationDataArray[(int)animationType].intMeshIDBlobArray, animationDataSO.frames.Length);
                for (int j = 0; j < animationDataSO.frames.Length; j++)
                {
                    blobBuilderMeshArray[j] = blodAssetDataDictionary[animationType][j];
                }
            }
            animationDataHolder.ValueRW.animationDataBlobArray = blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);
            //error memory leak
            blobBuilder.Dispose();
            //error unallocated memory
        }
    }
    [BurstCompile]
    public void onDestroy(ref SystemState state)
    {
        foreach (RefRW<AnimationDataHolder> animationDataHolder in SystemAPI.Query<RefRW<AnimationDataHolder>>())
        {
            animationDataHolder.ValueRW.animationDataBlobArray.Dispose();
        }
    }
}
