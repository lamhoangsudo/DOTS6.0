using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectVisualSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRO<Select> select in SystemAPI.Query<RefRO<Select>>())
        {
            RefRW<LocalTransform> visualSelectLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(select.ValueRO.visualEntity);
            visualSelectLocalTransform.ValueRW.Scale = select.ValueRO.scale;
        }
        foreach (RefRO<Select> select in SystemAPI.Query<RefRO<Select>>().WithDisabled<Select>())
        {
            RefRW<LocalTransform> visualSelectLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(select.ValueRO.visualEntity);
            visualSelectLocalTransform.ValueRW.Scale = 0f;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
