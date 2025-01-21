using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
//to set order systerm run 
//all order systerms need to run in the same group
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventSysterm))]
partial struct SelectVisualSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<Select> select in SystemAPI.Query<RefRO<Select>>().WithPresent<Select>())
        {
            
            if (select.ValueRO.OnSelect) 
            { 
                RefRW<LocalTransform> visualSelectLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(select.ValueRO.visualEntity);
                visualSelectLocalTransform.ValueRW.Scale = select.ValueRO.scale; 
            }
            if (select.ValueRO.OnDeSelect) 
            {
                RefRW<LocalTransform> visualSelectLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(select.ValueRO.visualEntity);
                visualSelectLocalTransform.ValueRW.Scale = 0f; 
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
