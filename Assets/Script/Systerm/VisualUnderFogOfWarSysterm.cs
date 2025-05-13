using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

partial struct VisualUnderFogOfWarSysterm : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((RefRW<VisualUnderFogOfWar> visualUnderFogOfWar, Entity entity) in SystemAPI.Query<RefRW<VisualUnderFogOfWar>>().WithEntityAccess())
        {
            Entity parentEntity = visualUnderFogOfWar.ValueRO.parentEntity;
            RefRO<LocalTransform> parentTransform = SystemAPI.GetComponentRO<LocalTransform>(parentEntity);
            if (collisionWorld.SphereCast(
                parentTransform.ValueRO.Position, 
                visualUnderFogOfWar.ValueRO.sphereCastSize, 
                new float3(0, 1, 0), 
                100,
                new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.FOG_OF_WAR,
                    GroupIndex = 0,
                }
                ))
            {
                //under visual fog of war, show the visual
                if(!visualUnderFogOfWar.ValueRO.isVisible)
                {
                    visualUnderFogOfWar.ValueRW.isVisible = true;
                    entityCommandBuffer.RemoveComponent<DisableRendering>(entity);
                }
            }
            else
            {
                //not under visual fog of war, hide the visual
                if (visualUnderFogOfWar.ValueRO.isVisible)
                {
                    visualUnderFogOfWar.ValueRW.isVisible = false;
                    entityCommandBuffer.AddComponent<DisableRendering>(entity);
                }
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
