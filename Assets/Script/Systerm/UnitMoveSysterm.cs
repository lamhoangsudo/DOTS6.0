using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static GridSysterm;
using static UnityEngine.EventSystems.EventTrigger;

partial struct UnitMoveSysterm : ISystem
{
    public const float REACH_TARGET_DISTANCE_SQ = 2f;
    [BurstCompile]
    public void onCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridSysterm.GridSystemData>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GridSysterm.GridSystemData gridSystemData = SystemAPI.GetSingleton<GridSysterm.GridSystemData>();
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;        
        EntityCommandBuffer entityCommandBuffer = new(Allocator.TempJob);
        TargetPositionPathQueuedJob targetPositionPathQueuedJob = new()
        {
            moveOverideLookup = SystemAPI.GetComponentLookup<MoveOveride>(isReadOnly: false),
            collisionWorld = collisionWorld,
            cellSize = gridSystemData.cellSize,
            width = gridSystemData.width,
            height = gridSystemData.height,
            gridCostArray = gridSystemData.gridCostArray,
            entityCommandBuffer = entityCommandBuffer.AsParallelWriter()
        };
        targetPositionPathQueuedJob.ScheduleParallel();
        state.Dependency.Complete();
        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
        /*
        foreach ((RefRO<LocalTransform> localTransform, RefRW<FlowFieldPathRequest> flowFieldPathRequest, EnabledRefRW<FlowFieldPathRequest> flowFieldPathRequestEnabled, RefRW<UnitMover> unitMover, RefRW<TargetPositionPathQueued> targetPositionPathQueued, EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled, EnabledRefRW<FlowFieldFollower> flowFieldFollowerEnabled, Entity entity)
            in
            SystemAPI.Query<RefRO<LocalTransform>, RefRW<FlowFieldPathRequest>, EnabledRefRW<FlowFieldPathRequest>, RefRW<UnitMover>, RefRW<TargetPositionPathQueued>, EnabledRefRW<TargetPositionPathQueued>, EnabledRefRW<FlowFieldFollower>>().WithPresent<FlowFieldPathRequest, FlowFieldFollower>().WithEntityAccess())
        {
            if (!collisionWorld.CastRay(new RaycastInput
            {
                Start = localTransform.ValueRO.Position,
                End = targetPositionPathQueued.ValueRO.targetPosition,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.PATHFINDING_WALL,
                    GroupIndex = 0
                }
            }))
            {
                unitMover.ValueRW.movePosition = targetPositionPathQueued.ValueRO.targetPosition;
                flowFieldPathRequestEnabled.ValueRW = false;
                flowFieldFollowerEnabled.ValueRW = false;
            }
            else
            {
                if (SystemAPI.HasComponent<MoveOveride>(entity))
                {
                    SystemAPI.SetComponentEnabled<MoveOveride>(entity, false);
                }
                if (GridSysterm.IsValidWalkable(targetPositionPathQueued.ValueRO.targetPosition, gridSystemData))
                {
                    flowFieldPathRequest.ValueRW.targetPosition = targetPositionPathQueued.ValueRO.targetPosition;
                    flowFieldPathRequestEnabled.ValueRW = true;
                }
                else
                {
                    unitMover.ValueRW.movePosition = localTransform.ValueRO.Position;
                    flowFieldPathRequestEnabled.ValueRW = false;
                    flowFieldFollowerEnabled.ValueRW = false;
                }
            }
            targetPositionPathQueuedEnabled.ValueRW = false;
        }
        */
        TestCanStraightJob testCanStraightJob = new()
        {
            collisionWorld = collisionWorld
        };
        testCanStraightJob.ScheduleParallel();
        FlowFieldFollowerJob flowFieldFollowerJob = new()
        {
            cellSize = gridSystemData.cellSize,
            width = gridSystemData.width,
            height = gridSystemData.height,
            gridEntityArray = gridSystemData.totalGridMapEntitys,
            gridNodeLookup = SystemAPI.GetComponentLookup<GridSysterm.Node>(isReadOnly: true)
        };
        flowFieldFollowerJob.ScheduleParallel();
        /*
        foreach ((RefRO<LocalTransform> localTransform, RefRW<FlowFieldFollower> flowFieldFollower, EnabledRefRW<FlowFieldFollower> flowFieldFollowerEnabled, RefRW<UnitMover> unitMover)
        in
        SystemAPI.Query<RefRO<LocalTransform>, RefRW<FlowFieldFollower>, EnabledRefRW<FlowFieldFollower>, RefRW<UnitMover>>())
        {
            int2 gridPosition = GridSysterm.GetGridPosition(localTransform.ValueRO.Position, gridSystemData.cellSize);
            int index = GridSysterm.CalculateIndex(gridPosition.x, gridPosition.y, gridSystemData.width);
            GridSysterm.Node node = SystemAPI.GetComponent<GridSysterm.Node>(gridSystemData.gridMaps[flowFieldFollower.ValueRO.gridIndex].gridEntityArray[index]);
            float3 moveVector = GridSysterm.GetWorldMovementVector(node.vector);
            if (GridSysterm.IsWall(node))
            {
                moveVector = flowFieldFollower.ValueRO.lastMoveVector;
            }
            else
            {
                flowFieldFollower.ValueRW.lastMoveVector = moveVector;
            }
            unitMover.ValueRW.movePosition = GridSysterm.GetCenterWorldPosition(gridPosition.x, gridPosition.y, gridSystemData.cellSize) + 2f * gridSystemData.cellSize * moveVector;
            if (math.distance(localTransform.ValueRO.Position, flowFieldFollower.ValueRO.targetPosition) < gridSystemData.cellSize)
            {
                unitMover.ValueRW.movePosition = flowFieldFollower.ValueRO.targetPosition;
                unitMover.ValueRW.isMoving = false;
            }
            
            if (!collisionWorld.CastRay(new RaycastInput
            {
                Start = localTransform.ValueRO.Position,
                End = flowFieldFollower.ValueRO.targetPosition,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.PATHFINDING_WALL,
                    GroupIndex = 0
                }
            }))
            {
                unitMover.ValueRW.movePosition = flowFieldFollower.ValueRO.targetPosition;
                flowFieldFollowerEnabled.ValueRW = false;
            }
            
        }
        */
        /*
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<UnitMover> unitMover)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<UnitMover>>())
        {
            float3 targetPosition = localTransform.ValueRO.Position + new float3(1, 0, 0);
            float3 dir = targetPosition - localTransform.ValueRO.Position;
            dir = math.normalize(dir);
            localTransform.ValueRW.Position += dir * unitMover.ValueRO.unitMover * SystemAPI.Time.DeltaTime;
            localTransform.ValueRW.Rotation = quaternion.LookRotation(dir, math.up());
        }
        */
        UnitMoverJob unitMoverJob = new()
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        unitMoverJob.ScheduleParallel();
        /*foreach ((
            RefRW<PhysicsVelocity> velocity,
            RefRW<LocalTransform> localTransform,
            RefRO<UnitMover> unitMover)
            in SystemAPI.Query<
                RefRW<PhysicsVelocity>,
                RefRW<LocalTransform>,
                RefRO<UnitMover>>())
        {
            float3 dir = unitMover.ValueRO.movePosition - localTransform.ValueRO.Position;
            dir = math.normalize(dir);
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRW.Rotation, quaternion.LookRotation(dir, math.up()), SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
            velocity.ValueRW.Linear = dir * unitMover.ValueRO.moveSpeed;
            velocity.ValueRW.Angular = float3.zero;
        }
        */
    }
}
[BurstCompile]
[WithPresent(typeof(FlowFieldPathRequest), typeof(FlowFieldFollower))]
[WithAll(typeof(TargetPositionPathQueued))]
public partial struct TargetPositionPathQueuedJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<MoveOveride> moveOverideLookup;
    [ReadOnly] public CollisionWorld collisionWorld;
    public float cellSize;
    public int width;
    public int height;
    [ReadOnly] public NativeArray<byte> gridCostArray;
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;
    public void Execute(
        in LocalTransform localTransform, 
        ref FlowFieldPathRequest flowFieldPathRequest, 
        EnabledRefRW<FlowFieldPathRequest> flowFieldPathRequestEnabled, 
        ref UnitMover unitMover, 
        ref TargetPositionPathQueued targetPositionPathQueued,
        EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled, 
        EnabledRefRW<FlowFieldFollower> flowFieldFollowerEnabled, 
        Entity entity)
    {
        if (!collisionWorld.CastRay(new RaycastInput
        {
            Start = localTransform.Position,
            End = targetPositionPathQueued.targetPosition,
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.PATHFINDING_WALL,
                GroupIndex = 0
            }
        }))
        {
            unitMover.movePosition = targetPositionPathQueued.targetPosition;
            flowFieldPathRequestEnabled.ValueRW = false;
            flowFieldFollowerEnabled.ValueRW = false;
        }
        else
        {
            if (moveOverideLookup.HasComponent(entity))
            {
                entityCommandBuffer.SetComponentEnabled<MoveOveride>(entity.Index, entity, false);
            }
            if (GridSysterm.IsValidWalkable(targetPositionPathQueued.targetPosition, cellSize, width, height, gridCostArray))
            {
                flowFieldPathRequest.targetPosition = targetPositionPathQueued.targetPosition;
                flowFieldPathRequestEnabled.ValueRW = true;
            }
            else
            {
                unitMover.movePosition = localTransform.Position;
                flowFieldPathRequestEnabled.ValueRW = false;
                flowFieldFollowerEnabled.ValueRW = false;
            }
        }
        targetPositionPathQueuedEnabled.ValueRW = false;
    }
}
[BurstCompile]
[WithAll(typeof(FlowFieldFollower))]
public partial struct TestCanStraightJob : IJobEntity
{
    [ReadOnly] public CollisionWorld collisionWorld;
    public void Execute(
        in LocalTransform localTransform,
        ref FlowFieldFollower flowFieldFollower,
        EnabledRefRW<FlowFieldFollower> flowFieldFollowerEnabled,
        ref UnitMover unitMover)
    {
        if (!collisionWorld.CastRay(new RaycastInput
        {
            Start = localTransform.Position,
            End = flowFieldFollower.targetPosition,
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.PATHFINDING_WALL,
                GroupIndex = 0
            }
        }))
        {
            unitMover.movePosition = flowFieldFollower.targetPosition;
            flowFieldFollowerEnabled.ValueRW = false;
        }
    }
}
[BurstCompile]
[WithAll(typeof(FlowFieldFollower))]
public partial struct FlowFieldFollowerJob : IJobEntity
{
    public float cellSize;
    public int width;
    public int height;
    [ReadOnly] public NativeArray<Entity> gridEntityArray;
    [ReadOnly] public ComponentLookup<GridSysterm.Node> gridNodeLookup;
    private void Execute(
        in LocalTransform localTransform, 
        ref FlowFieldFollower flowFieldFollower, 
        EnabledRefRW<FlowFieldFollower> flowFieldFollowerEnabled, 
        ref UnitMover unitMover)
    {
        int2 gridPosition = GridSysterm.GetGridPosition(localTransform.Position, cellSize);
        int index = GridSysterm.CalculateIndex(gridPosition.x, gridPosition.y, width);
        Entity gridEntityNode = gridEntityArray[(width * height * flowFieldFollower.gridIndex) + index];
        GridSysterm.Node node = gridNodeLookup[gridEntityNode];
        float3 moveVector = GridSysterm.GetWorldMovementVector(node.vector);
        if (GridSysterm.IsWall(node))
        {
            moveVector = flowFieldFollower.lastMoveVector;
        }
        else
        {
            flowFieldFollower.lastMoveVector = moveVector;
        }
        unitMover.movePosition = GridSysterm.GetCenterWorldPosition(gridPosition.x, gridPosition.y, cellSize) + 2f * cellSize * moveVector;
        if (math.distance(localTransform.Position, flowFieldFollower.targetPosition) < cellSize)
        {
            unitMover.movePosition = flowFieldFollower.targetPosition;
            unitMover.isMoving = false;
        }
    }
}

