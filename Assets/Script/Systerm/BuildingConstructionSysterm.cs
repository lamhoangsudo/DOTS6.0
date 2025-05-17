using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BuildingConstructionSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((RefRO<LocalTransform> localTransform, RefRW<BuildingContruction> buildingContruction, Entity entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<BuildingContruction>>().WithEntityAccess())
        {
            RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(buildingContruction.ValueRO.visualEntity);
            visualTransform.ValueRW.Position = math.lerp(buildingContruction.ValueRO.startPosition, buildingContruction.ValueRO.endPosition, buildingContruction.ValueRO.progress / buildingContruction.ValueRO.maxProgress);
            buildingContruction.ValueRW.progress += SystemAPI.Time.DeltaTime;
            if(buildingContruction.ValueRO.progress >= buildingContruction.ValueRO.maxProgress)
            {
                //construct the building
                Entity spawnedBuildingEntity = entityCommandBuffer.Instantiate(buildingContruction.ValueRO.finalPrefabEntity);
                entityCommandBuffer.SetComponent(spawnedBuildingEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                entityCommandBuffer.DestroyEntity(buildingContruction.ValueRO.visualEntity);
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
