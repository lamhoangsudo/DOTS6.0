using Unity.Burst;
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
        float3 cameraVector = float3.zero;
        if (Camera.main != null)
        {
            cameraVector = (float3)Camera.main.transform.forward;
        }
        foreach ((RefRO<HealthBar> healthBar, RefRW<LocalTransform> localTranform) in SystemAPI.Query<RefRO<HealthBar>, RefRW<LocalTransform>>())
        {
            if (localTranform.ValueRO.Scale != 0f)
            {
                RefRO<LocalTransform> parentLocalTranform = SystemAPI.GetComponentRO<LocalTransform>(healthBar.ValueRO.healthEntity);
                localTranform.ValueRW.Rotation = parentLocalTranform.ValueRO.InverseTransformRotation(quaternion.LookRotation(cameraVector, math.up()));
            }

            RefRO<Health> healthEntity = SystemAPI.GetComponentRO<Health>(healthBar.ValueRO.healthEntity);
            if (!healthEntity.ValueRO.OnValueHealthChange) continue;

            float healthNormalize = (float)healthEntity.ValueRO.health / healthEntity.ValueRO.healthMax;
            if (healthNormalize == 1f)
            {
                localTranform.ValueRW.Scale = 0f;
            }
            else
            {
                localTranform.ValueRW.Scale = 1f;
            }
            //RefRW<LocalTransform> healthBarLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(healthBar.ValueRO.barVisual);
            //healthBarLocalTransform.ValueRW.Scale = healthNormalize;
            //edit custom scale of axes
            RefRW<PostTransformMatrix> healthBarPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisual);
            healthBarPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalize, 1, 1);

        }
    }
}
