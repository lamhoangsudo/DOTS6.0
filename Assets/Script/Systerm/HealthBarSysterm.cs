using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //float3 cameraVector = float3.zero;
        //if (Camera.main != null)
        //{
        //    cameraVector = (float3)Camera.main.transform.forward;
        //}
        EntityCommandBuffer entityCommandBuffer = new(Allocator.TempJob);
        HealthBarjob healthBarjob = new()
        {
            componentLookupLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: false),
            componentLookupHealth = SystemAPI.GetComponentLookup<Health>(true),
            componentLookupPostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(isReadOnly: false),
            entityCommandBuffer = entityCommandBuffer.AsParallelWriter()
        }; 
        healthBarjob.ScheduleParallel();
        state.Dependency.Complete();
        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
        //foreach ((RefRO<HealthBar> healthBar, RefRW<LocalTransform> localTranform) in SystemAPI.Query<RefRO<HealthBar>, RefRW<LocalTransform>>())
        //{
        //    if (localTranform.ValueRO.Scale != 0f)
        //    {
        //        RefRO<LocalTransform> parentLocalTranform = SystemAPI.GetComponentRO<LocalTransform>(healthBar.ValueRO.healthEntity);
        //        localTranform.ValueRW.Rotation = parentLocalTranform.ValueRO.InverseTransformRotation(quaternion.LookRotation(healthBar.ValueRO.cameraVector, math.up()));
        //    }

        //    RefRO<Health> healthEntity = SystemAPI.GetComponentRO<Health>(healthBar.ValueRO.healthEntity);
        //    if (!healthEntity.ValueRO.OnValueHealthChange) continue;

        //    float healthNormalize = (float)healthEntity.ValueRO.health / healthEntity.ValueRO.healthMax;
        //    if (healthNormalize == 1f)
        //    {
        //        localTranform.ValueRW.Scale = 0f;
        //    }
        //    else
        //    {
        //        localTranform.ValueRW.Scale = 1f;
        //    }
        //    //RefRW<LocalTransform> healthBarLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(healthBar.ValueRO.barVisual);
        //    //healthBarLocalTransform.ValueRW.Scale = healthNormalize;
        //    //edit custom scale of axes
        //    RefRW<PostTransformMatrix> healthBarPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisual);
        //    healthBarPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalize, 1, 1);
        //}
    }
}
[BurstCompile]
public partial struct HealthBarjob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> componentLookupLocalTransform;
    [ReadOnly] public ComponentLookup<Health> componentLookupHealth;
    [ReadOnly] public ComponentLookup<PostTransformMatrix> componentLookupPostTransformMatrix;
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;
    private Entity healthEntity;
    private Entity barVisual;
    public void Execute([ChunkIndexInQuery] int sortKey, in HealthBar healthBar, in Entity entity)
    {
        healthEntity = healthBar.healthEntity;
        barVisual = healthBar.barVisual;
        LocalTransform localTransform = componentLookupLocalTransform[entity];
        PostTransformMatrix postTransformMatrixWrite = componentLookupPostTransformMatrix[barVisual];
        LocalTransform localTransformWrite = componentLookupLocalTransform[entity];
        if (localTransform.Scale != 0f)
        {
            RefRO<LocalTransform> parentLocalTranform = componentLookupLocalTransform.GetRefRO(healthEntity);
            localTransformWrite.Rotation = parentLocalTranform.ValueRO.InverseTransformRotation(quaternion.LookRotation(healthBar.cameraVector, math.up()));
            entityCommandBuffer.SetComponent<LocalTransform>(sortKey, entity, localTransformWrite);
        }

        Health health = componentLookupHealth[healthEntity];
        if (!health.OnValueHealthChange) return;

        float healthNormalize = (float)health.health / health.healthMax;
        if (healthNormalize == 1f)
        {
            localTransformWrite.Scale = 0f;
        }
        else
        {
            localTransformWrite.Scale = 1f;            
        }
        entityCommandBuffer.SetComponent<LocalTransform>(sortKey, entity, localTransformWrite);
        postTransformMatrixWrite.Value = float4x4.Scale(healthNormalize, 1, 1);
        entityCommandBuffer.SetComponent<PostTransformMatrix>(sortKey, barVisual, postTransformMatrixWrite);
    }
}
