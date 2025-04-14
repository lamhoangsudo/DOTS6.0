using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class AnimationDataHolderAuthoring : MonoBehaviour
{
    [SerializeField] private AnimationDataListSO listAnimationData;
    [SerializeField] private Material defaultMaterial;
    public class AnimationDataHolderAuthoringBaker : Baker<AnimationDataHolderAuthoring>
    {
        public override void Bake(AnimationDataHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AnimationDataHolder animationDataHolder = new();
            
            // when subscene is closed, entitiesGraphicsSystem and defaultGameObjectInjectionWorld is null
            /*
            EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
            */
            /*
            //
            BlobBuilder blobBuilder = new(Allocator.Temp);
            //error work with copy not actually data
            ref BlobArray<AnimationData> animationDataBlobArray = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();
            BlobBuilderArray<AnimationData> blobBuilderAnimationDataArray = blobBuilder.Allocate<AnimationData>(ref animationDataBlobArray, System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)).Length);
            //
            */
            foreach (AnimationDataSO.AnimationType animationType in System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)))
            {
                AnimationDataSO animationDataSO = authoring.listAnimationData.GetAnimationDataSO(animationType);
                /*
                blobBuilderAnimationDataArray[(int)animationType].frameTimeMax = animationDataSO.frameTimerMax;
                blobBuilderAnimationDataArray[(int)animationType].frameMax = animationDataSO.frames.Length;
                BlobBuilderArray<BatchMeshID> blobBuilderMeshArray = blobBuilder.Allocate<BatchMeshID>(ref blobBuilderAnimationDataArray[(int)animationType].intMeshIDBlobArray, animationDataSO.frames.Length);
                */
                for (int j = 0; j < animationDataSO.frames.Length; j++)
                {
                    /*
                    BatchMeshID batchMeshID = new();
                    batchMeshID = entitiesGraphicsSystem.RegisterMesh(animationDataSO.frames[j]);
                    blobBuilderMeshArray[j] = batchMeshID;
                    */
                    Entity additionalEntity = CreateAdditionalEntity(TransformUsageFlags.None, true);
                    AddComponent(additionalEntity, new MaterialMeshInfo());
                    AddComponent(additionalEntity, new RenderMeshUnmanaged
                    {
                        materialForSubMesh = authoring.defaultMaterial,
                        mesh = animationDataSO.frames[j],
                    });
                    AddComponent(additionalEntity, new AnimationDataSubEntity
                    {
                        animationType = animationType,
                        frame = j,
                    });
                }
            }
            /*
            //
            animationDataHolder.animationDataBlobArray = blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);
            //error memory leak
            blobBuilder.Dispose();
            //error unallocated memory
            AddBlobAsset(ref animationDataHolder.animationDataBlobArray, out _);
            */
            AddComponent(entity, new AnimationDataHolderObject
            {
                animationDataListSO = authoring.listAnimationData,
            });
            AddComponent(entity, animationDataHolder);
            
        }
    }
}


