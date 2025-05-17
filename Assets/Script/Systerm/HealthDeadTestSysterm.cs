using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct HealthDeadTestSysterm : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //buid entity commander buffer
        //best way
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        //EntityCommandBuffer entityCommandBuffer = new(Allocator.Temp);
        //get access entity 
        foreach ((RefRW<Health> entityHealth, 
            Entity entity) 
            in 
            SystemAPI.Query<RefRW<Health>>()
            .WithEntityAccess())
        {
            if(entityHealth.ValueRO.health <= 0)
            {
                //get error when destroy entity 
                //because cause structural change
                //state.EntityManager.DestroyEntity(entity);
                //using entity commander buffer to save action destroy entity
                //excute later
                entityHealth.ValueRW.OnDead = true;
                entityCommandBuffer.DestroyEntity(entity);
                if(SystemAPI.HasComponent<BuildingContruction>(entity))
                {
                    entityCommandBuffer.DestroyEntity(SystemAPI.GetComponent<BuildingContruction>(entity).visualEntity);
                }
            }
        }
        //entityCommandBuffer.Playback(state.EntityManager);
        //auto call the end of frame
    }
}
