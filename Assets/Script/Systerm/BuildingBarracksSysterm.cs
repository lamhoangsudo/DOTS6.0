using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct BuildingBarracksSysterm : ISystem
{
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntityReferenecs>();
    }
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityReferenecs entityReferenecs = SystemAPI.GetSingleton<EntityReferenecs>();
        foreach((
            RefRO<BuildingBarracksUnitEnqueue> buildingBarracksUnitEnqueue, 
            EnabledRefRW<BuildingBarracksUnitEnqueue> buildingBarracksUnitEnqueueEnabled,
            DynamicBuffer<SpawnUnitType> spawnUnits, 
            RefRW<BuildingBarracks> buildingBarracks) 
            in
            SystemAPI.Query<
                RefRO<BuildingBarracksUnitEnqueue>, 
                EnabledRefRW<BuildingBarracksUnitEnqueue>, 
                DynamicBuffer<SpawnUnitType>, 
                RefRW<BuildingBarracks>>())
        {
            spawnUnits.Add(new SpawnUnitType
            {
                unitType = buildingBarracksUnitEnqueue.ValueRO.unitType,
            });
            buildingBarracksUnitEnqueueEnabled.ValueRW = false;
            buildingBarracks.ValueRW.onUnitQueueChanged = true;
        } 
        foreach ((RefRW<BuildingBarracks> buildingBarracks, RefRO<LocalTransform> localTransform, DynamicBuffer<SpawnUnitType> spawnUnitTypes) in SystemAPI.Query<RefRW<BuildingBarracks>, RefRO<LocalTransform>, DynamicBuffer<SpawnUnitType>>())
        {
            if(spawnUnitTypes.IsEmpty)
            {
                continue;
            }
            if(buildingBarracks.ValueRO.activeUnitType != spawnUnitTypes[0].unitType)
            {
                buildingBarracks.ValueRW.activeUnitType = spawnUnitTypes[0].unitType;
                buildingBarracks.ValueRW.maxProgress = GameAssets.instance.unitTypeListSO.GetUnitTypeSO(buildingBarracks.ValueRO.activeUnitType).progress;
            }
            buildingBarracks.ValueRW.progress += SystemAPI.Time.DeltaTime;
            if(buildingBarracks.ValueRW.progress < buildingBarracks.ValueRO.maxProgress)
            {
                continue;
            }
            buildingBarracks.ValueRW.progress = 0f;
            UnitTypeSO.UnitType unitType = spawnUnitTypes[0].unitType;
            UnitTypeSO unitTypeSO = GameAssets.instance.unitTypeListSO.GetUnitTypeSO(unitType);
            unitTypeSO.GetPrefabEntity(entityReferenecs);
            spawnUnitTypes.RemoveAt(0);
            buildingBarracks.ValueRW.onUnitQueueChanged = true;
            Entity entity = state.EntityManager.Instantiate(unitTypeSO.GetPrefabEntity(entityReferenecs));
            SystemAPI.SetComponent<LocalTransform>(entity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
            SystemAPI.SetComponent(entity, new MoveOveride
            {
                targetPosition = localTransform.ValueRO.Position + buildingBarracks.ValueRO.rallyPositionOffset,
            });
            SystemAPI.SetComponentEnabled<MoveOveride>(entity, true);
        }
    }
}
