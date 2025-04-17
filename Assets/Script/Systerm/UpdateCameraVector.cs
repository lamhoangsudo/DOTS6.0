using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[UpdateBefore(typeof(HealthBarSysterm))]
partial class UpdateCameraVector : SystemBase
{
    protected override void OnUpdate()
    {
        foreach(RefRW<HealthBar> healthBar in SystemAPI.Query<RefRW<HealthBar>>())
        {
            healthBar.ValueRW.cameraVector = Camera.main != null ? Camera.main.transform.forward : float3.zero;
        }
    }
}
