using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BuildingBarracksAuthoring : MonoBehaviour
{
    public float maxProgress;
    public Vector3 rallyPositionOffset;
    public class BuildingBarracksAuthoringBaker : Baker<BuildingBarracksAuthoring>
    {
        public override void Bake(BuildingBarracksAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingBarracks
            {
                maxProgress = authoring.maxProgress,
                rallyPositionOffset = authoring.rallyPositionOffset,
            });
            DynamicBuffer<SpawnUnitType> spawnUnitTypes = AddBuffer<SpawnUnitType>(entity);
            spawnUnitTypes.Add(new SpawnUnitType
            {
                unitType = UnitTypeSO.UnitType.Soldier,
            });
            spawnUnitTypes.Add(new SpawnUnitType
            {
                unitType = UnitTypeSO.UnitType.Scout,
            }); 
            spawnUnitTypes.Add(new SpawnUnitType
            {
                unitType = UnitTypeSO.UnitType.Soldier,
            });
            spawnUnitTypes.Add(new SpawnUnitType
            {
                unitType = UnitTypeSO.UnitType.Scout,
            });
        }
    }
}
public struct BuildingBarracks : IComponentData
{
    public float progress;
    public float maxProgress;
    public UnitTypeSO.UnitType activeUnitType;
    public float3 rallyPositionOffset;
}
//the number is fixed to what we need, lager causes performance issues
[InternalBufferCapacity(10)]
public struct SpawnUnitType : IBufferElementData
{
    public UnitTypeSO.UnitType unitType;
}


