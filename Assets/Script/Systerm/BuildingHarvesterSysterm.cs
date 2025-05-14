using Unity.Burst;
using Unity.Entities;

partial struct BuildingHarvesterSysterm : ISystem
{
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRW<BuildingHarvester> buildingHarvester in SystemAPI.Query<RefRW<BuildingHarvester>>())
        {
            buildingHarvester.ValueRW.harvestTime -= SystemAPI.Time.DeltaTime;
            if(buildingHarvester.ValueRO.harvestTime <= 0)
            {
                buildingHarvester.ValueRW.harvestTime = buildingHarvester.ValueRO.harvestTimeMax;
                ResourceManager.Instance.AddResourceAmount(buildingHarvester.ValueRO.resourceType, 1);
            }
        }
    }
}
