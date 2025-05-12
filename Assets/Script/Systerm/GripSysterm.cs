//#define GRID_DEBUG
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using static GridSysterm;

public partial struct GridSysterm : ISystem
{
    private const int WALL_COST = byte.MaxValue, HEAVY_COST = 50;
    private const int FLOW_FIELD_MAP_COUNT = 50;
    public struct GridSystemData : IComponentData
    {
        public int width;
        public int height;
        public float cellSize;
        public NativeArray<GridMap> gridMaps;
        public NativeArray<byte> gridCostArray;
        public NativeArray<Entity> totalGridMapEntitys;
        public int nextGridIndex;
    }
    public struct Node : IComponentData
    {
        public int gridIndex;
        public int x;
        public int y;
        public byte cost;
        public int bestCost;
        public float2 vector;
    }
    public struct GridMap
    {
        public NativeArray<Entity> gridEntityArray;
        public int2 targetGridPosition;
        public bool isValid;
    }
#if !GRID_DEBUG
    [BurstCompile]
#endif
    public void OnCreate(ref SystemState state)
    {
        int width = 10;
        int height = 10;
        float cellSize = 5.0f;
        int totalCells = width * height;
        Entity entityNode = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponent<Node>(entityNode);
        NativeArray<GridMap> gridMaps = new NativeArray<GridMap>(FLOW_FIELD_MAP_COUNT, Allocator.Persistent);
        NativeList<Entity> totalGridMapEntitys = new NativeList<Entity>(totalCells * FLOW_FIELD_MAP_COUNT, Allocator.Temp);
        for (int i = 0; i < FLOW_FIELD_MAP_COUNT; i++)
        {
            GridMap gridMap = new GridMap
            {
                gridEntityArray = new NativeArray<Entity>(totalCells, Allocator.Persistent),
                isValid = false,
            };
            state.EntityManager.Instantiate(entityNode, gridMap.gridEntityArray);
            totalGridMapEntitys.AddRange(gridMap.gridEntityArray);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = CalculateIndex(x, y, width);
                    Entity entity = gridMap.gridEntityArray[index];
#if GRID_DEBUG
                    state.EntityManager.SetName(entity, $"Node_{x}_{y}");
#endif
                    SystemAPI.SetComponent<Node>(entity, new Node
                    {
                        gridIndex = i,
                        x = x,
                        y = y,
                    });
                }
            }
            gridMaps[i] = gridMap;
        }
        state.EntityManager.AddComponent<GridSystemData>(state.SystemHandle);
        state.EntityManager.SetComponentData<GridSystemData>(state.SystemHandle, new GridSystemData
        {
            width = width,
            height = height,
            cellSize = cellSize,
            gridMaps = gridMaps,
            gridCostArray = new NativeArray<byte>(totalCells, Allocator.Persistent),
            totalGridMapEntitys = totalGridMapEntitys.ToArray(Allocator.Persistent),
        });
        totalGridMapEntitys.Dispose();
    }
#if !GRID_DEBUG
    [BurstCompile]
#endif
    public void OnUpdate(ref SystemState state)
    {
        GridSystemData gridSystermData = SystemAPI.GetComponent<GridSystemData>(state.SystemHandle);

        foreach ((RefRW<FlowFieldPathRequest> flowFieldPathRequest, EnabledRefRW<FlowFieldPathRequest> flowFieldPathRequestEnabled, RefRW<FlowFieldFollower> flowFieldFollower, EnabledRefRW<FlowFieldFollower> flowFieldFollowerEnabled)
            in
            SystemAPI.Query<RefRW<FlowFieldPathRequest>, EnabledRefRW<FlowFieldPathRequest>, RefRW<FlowFieldFollower>, EnabledRefRW<FlowFieldFollower>>().WithPresent<FlowFieldFollower>())
        {
            int2 targetGridPosition = GetGridPosition(flowFieldPathRequest.ValueRO.targetPosition, gridSystermData.cellSize);
            flowFieldPathRequestEnabled.ValueRW = false;
            bool isCaculated = false;
            for (int i = 0; i < FLOW_FIELD_MAP_COUNT; i++)
            {
                if (gridSystermData.gridMaps[i].isValid && gridSystermData.gridMaps[i].targetGridPosition.Equals(targetGridPosition))
                {
                    flowFieldFollower.ValueRW.targetPosition = flowFieldPathRequest.ValueRO.targetPosition;
                    flowFieldFollower.ValueRW.gridIndex = i;
                    flowFieldFollowerEnabled.ValueRW = true;
                    isCaculated = true;
                    break;
                }
            }
            if (isCaculated) continue;
            int gridIndex = gridSystermData.nextGridIndex;
            gridSystermData.nextGridIndex = (gridSystermData.nextGridIndex + 1) % FLOW_FIELD_MAP_COUNT;
            flowFieldFollower.ValueRW.targetPosition = flowFieldPathRequest.ValueRO.targetPosition;
            flowFieldFollower.ValueRW.gridIndex = gridIndex;
            flowFieldFollowerEnabled.ValueRW = true;
            NativeArray<RefRW<GridSysterm.Node>> nodeArray = new(gridSystermData.gridMaps[gridIndex].gridEntityArray.Length, Allocator.Temp);
            InitializeGridJob initializeGridJob = new InitializeGridJob
            {
                gridIndex = gridIndex,
                targetGridPosition = targetGridPosition
            };
            JobHandle initializeGridJobHandle = initializeGridJob.ScheduleParallel(state.Dependency);
            initializeGridJobHandle.Complete();
            for (int x = 0; x < gridSystermData.width; x++)
            {
                for (int y = 0; y < gridSystermData.height; y++)
                {
                    int index = CalculateIndex(x, y, gridSystermData.width);
                    Entity nodeEntity = gridSystermData.gridMaps[gridIndex].gridEntityArray[index];
                    RefRW<GridSysterm.Node> node = SystemAPI.GetComponentRW<GridSysterm.Node>(nodeEntity);
                    nodeArray[index] = node;
#if GRID_DEBUG
                    if(GetGridPositionFromIndex(index, gridSystermData.width).Equals(targetGridPosition))
                    {
                        Debug.Log($"Node {x}, {y} is a end node and cost: {node.ValueRO.cost} and best cost: {node.ValueRO.bestCost}");
                    }
#endif
                    //node.ValueRW.vector = new float2(0, 1);
                    //if (x == targetGridPosition.x && y == targetGridPosition.y)
                    //{
                    //    node.ValueRW.cost = 0;
                    //    node.ValueRW.bestCost = 0;
                    //}
                    //else
                    //{
                    //    node.ValueRW.cost = 1;
                    //    node.ValueRW.bestCost = int.MaxValue;
                    //}
                }
            }
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            //EntityCommandBuffer entityCommandBufferUpdateCostMapJob = new(Allocator.TempJob);
            UpdateCostMapJob updateCostMapJob = new UpdateCostMapJob
            {
                width = gridSystermData.width,
                cellSize = gridSystermData.cellSize,
                cellSizeHaft = gridSystermData.cellSize * 0.5f,
                collisionFilterWall = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.PATHFINDING_WALL,
                    GroupIndex = 0
                },
                collisionFilterHeavy = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.PATHFINDING_HEAVY,
                    GroupIndex = 0
                },
                gridMap = gridSystermData.gridMaps[gridIndex],
                collisionWorld = collisionWorld,
                gridCostArray = gridSystermData.gridCostArray,
                //entityCommandBuffer = entityCommandBufferUpdateCostMapJob.AsParallelWriter()
            };
            JobHandle updateCostMapJobHandle = updateCostMapJob.ScheduleParallel(state.Dependency);
            updateCostMapJobHandle.Complete();
            //entityCommandBufferUpdateCostMapJob.Playback(state.EntityManager);
            //entityCommandBufferUpdateCostMapJob.Dispose();
            //for (int x = 0; x < gridSystermData.width; x++)
            //{
            //    for (int y = 0; y < gridSystermData.height; y++)
            //    {
            //        if (collisionWorld.OverlapSphere(GetCenterWorldPosition(x, y, gridSystermData.cellSize),
            //            gridSystermData.cellSize * 0.5f,
            //            ref distanceHitList,
            //            new CollisionFilter
            //            {
            //                BelongsTo = ~0u,
            //                CollidesWith = 1u << GameAssets.PATHFINDING_WALL,
            //                GroupIndex = 0
            //            }))
            //        {
            //            int index = CalculateIndex(x, y, gridSystermData.width);
            //            nodeArray[index].ValueRW.cost = WALL_COST;
            //            gridSystermData.gridCostArray[index] = WALL_COST;
            //        }
            //        if (collisionWorld.OverlapSphere(GetCenterWorldPosition(x, y, gridSystermData.cellSize),
            //            gridSystermData.cellSize * 0.5f,
            //            ref distanceHitList,
            //            new CollisionFilter
            //            {
            //                BelongsTo = ~0u,
            //                CollidesWith = 1u << GameAssets.PATHFINDING_HEAVY,
            //                GroupIndex = 0
            //            }))
            //        {
            //            int index = CalculateIndex(x, y, gridSystermData.width);
            //            nodeArray[index].ValueRW.cost = HEAVY_COST;
            //            gridSystermData.gridCostArray[index] = HEAVY_COST;
            //        }
            //    }
            //}

            NativeQueue<RefRW<GridSysterm.Node>> gridOpenNodeQueue = new(Allocator.Temp);
            RefRW<GridSysterm.Node> startNode = nodeArray[CalculateIndex(targetGridPosition.x, targetGridPosition.y, gridSystermData.width)];
            gridOpenNodeQueue.Enqueue(startNode);
            while (gridOpenNodeQueue.Count > 0)
            {
                RefRW<GridSysterm.Node> currentNode = gridOpenNodeQueue.Dequeue();
                NativeList<RefRW<GridSysterm.Node>> neighborNodeList = GetNeighborGridNodeList(currentNode, nodeArray, gridSystermData);
                foreach (RefRW<GridSysterm.Node> neighborNode in neighborNodeList)
                {
                    if (neighborNode.ValueRW.cost == WALL_COST) continue;
                    if (neighborNode.ValueRW.bestCost > currentNode.ValueRW.bestCost + neighborNode.ValueRW.cost)
                    {
                        neighborNode.ValueRW.bestCost = currentNode.ValueRW.bestCost + neighborNode.ValueRW.cost;
                        neighborNode.ValueRW.vector = CaculateVector(new int2(neighborNode.ValueRW.x, neighborNode.ValueRW.y), new int2(currentNode.ValueRW.x, currentNode.ValueRW.y));
                        gridOpenNodeQueue.Enqueue(neighborNode);
                    }
                }
                neighborNodeList.Dispose();
            }
            gridOpenNodeQueue.Dispose();
            nodeArray.Dispose();
            GridMap gridMap = gridSystermData.gridMaps[gridIndex];
            gridMap.targetGridPosition = targetGridPosition;
            gridMap.isValid = true;
            gridSystermData.gridMaps[gridIndex] = gridMap;
            SystemAPI.SetComponent<GridSystemData>(state.SystemHandle, gridSystermData);
        }

#if GRID_DEBUG
        GridSystermDebug.Instance?.InitializedGrid(gridSystermData);
        GridSystermDebug.Instance?.UpdateGrid(gridSystermData);
#endif
    }

#if !GRID_DEBUG
    [BurstCompile]
#endif
    public void OnDestroy(ref SystemState state)
    {
        RefRW<GridSystemData> gridSystemData = SystemAPI.GetComponentRW<GridSystemData>(state.SystemHandle);
        foreach (GridMap gridMap in gridSystemData.ValueRO.gridMaps)
        {
            gridMap.gridEntityArray.Dispose();

        }
        gridSystemData.ValueRW.gridMaps.Dispose();
        gridSystemData.ValueRW.gridCostArray.Dispose();
        gridSystemData.ValueRW.totalGridMapEntitys.Dispose();
    }
    public static float2 CaculateVector(int2 start, int2 end)
    {
        return new float2(end.x - start.x, end.y - start.y);
    }
    public static NativeList<RefRW<GridSysterm.Node>> GetNeighborGridNodeList(RefRW<GridSysterm.Node> currentNode, NativeArray<RefRW<GridSysterm.Node>> nodeArray, GridSystemData gridSystemData)
    {
        NativeList<RefRW<GridSysterm.Node>> neighborNodeList = new NativeList<RefRW<GridSysterm.Node>>(Allocator.Temp);
        int2 currentGridPosition = new int2(currentNode.ValueRW.x, currentNode.ValueRW.y);
        int2[] neighborOffsets = new int2[]
        {
            new int2(0, 1), // Up
            new int2(1, 0), // Right
            new int2(0, -1), // Down
            new int2(-1, 0), // Left
            new int2(1, 1), // Up-Right
            new int2(1, -1), // Down-Right
            new int2(-1, 1), // Up-Left
            new int2(-1, -1) // Down-Left
        };
        foreach (int2 offset in neighborOffsets)
        {
            int2 neighborGridPosition = currentGridPosition + offset;
            if (!IsInGrid(neighborGridPosition, gridSystemData.width, gridSystemData.height)) continue;
            neighborNodeList.Add(nodeArray[CalculateIndex(neighborGridPosition.x, neighborGridPosition.y, gridSystemData.width)]);
        }
        return neighborNodeList;
    }
    public static int CalculateIndex(int x, int y, int width)
    {
        return x + y * width;
    }
    public static int2 GetGridPositionFromIndex(int index, int width)
    {
        int y = (int)math.floor(index / width);
        int x = index % width;
        return new int2(x, y);
    }
    public static float3 GetWorldPosition(int x, int y, float cellSize)
    {
        return new float3(x * cellSize, 0, y * cellSize);
    }
    public static float3 GetCenterWorldPosition(int x, int y, float cellSize)
    {
        return new float3(x * cellSize + cellSize * 0.5f, 0, y * cellSize + cellSize * 0.5f);
    }
    public static int2 GetGridPosition(float3 position, float cellsize)
    {
        return new int2((int)math.floor(position.x / cellsize), (int)math.floor(position.z / cellsize));
    }
    public static bool IsInGrid(int2 gridPosition, int width, int height)
    {
        return gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height;
    }
    public static float3 GetWorldMovementVector(float2 vector)
    {
        return new float3(vector.x, 0, vector.y);
    }
    public static bool IsWall(GridSysterm.Node node)
    {
        return node.cost == WALL_COST;
    }
    public static bool IsWall(int2 gridPosition, GridSystemData gridSystemData)
    {
        return gridSystemData.gridCostArray[CalculateIndex(gridPosition.x, gridPosition.y, gridSystemData.width)] == WALL_COST;
    }
    public static bool IsWall(int2 gridPosition, int width, NativeArray<byte> gridCostArray)
    {
        return gridCostArray[CalculateIndex(gridPosition.x, gridPosition.y, width)] == WALL_COST;
    }
    public static bool IsValidWalkable(float3 position, GridSystemData gridSystemData)
    {
        int2 gridPosition = GetGridPosition(position, gridSystemData.cellSize);
        return IsInGrid(gridPosition, gridSystemData.width, gridSystemData.height) && !IsWall(gridPosition, gridSystemData);
    }
    public static bool IsValidWalkable(float3 position, float cellSize, int width, int height, NativeArray<byte> gridCostArray)
    {
        int2 gridPosition = GetGridPosition(position, cellSize);
        return IsInGrid(gridPosition, width, height) && !IsWall(gridPosition, width, gridCostArray);
    }
    [BurstCompile]
    public partial struct InitializeGridJob : IJobEntity
    {
        public int gridIndex;
        public int2 targetGridPosition;
        public void Execute(ref GridSysterm.Node node)
        {
            if (node.gridIndex != gridIndex) return;
            node.vector = new float2(0, 1);
            if (node.x == targetGridPosition.x && node.y == targetGridPosition.y)
            {
                node.cost = 0;
                node.bestCost = 0;
            }
            else
            {
                node.cost = 1;
                node.bestCost = int.MaxValue;
            }
        }
    }
    [BurstCompile]
    public partial struct UpdateCostMapJob : IJobEntity
    {
        [ReadOnly] public int width;
        [ReadOnly] public float cellSize;
        [ReadOnly] public float cellSizeHaft;
        [ReadOnly] public CollisionFilter collisionFilterWall;
        [ReadOnly] public CollisionFilter collisionFilterHeavy;
        [ReadOnly] public GridMap gridMap;
        [ReadOnly] public CollisionWorld collisionWorld;

        [NativeDisableParallelForRestriction] public NativeArray<byte> gridCostArray;

        //public EntityCommandBuffer.ParallelWriter entityCommandBuffer;
        public void Execute(ref GridSysterm.Node node)
        {
            NativeList<DistanceHit> distanceHitList = new(Allocator.TempJob);
            byte cost = 0;
            if (collisionWorld.OverlapSphere(GetCenterWorldPosition(node.x, node.y, cellSize),
                cellSizeHaft,
                ref distanceHitList,
                collisionFilterWall))
            {
                node.cost = WALL_COST;
                gridCostArray[CalculateIndex(node.x, node.y, width)] = cost;
            }
            if (collisionWorld.OverlapSphere(GetCenterWorldPosition(node.x, node.y, cellSize),
                cellSizeHaft,
                ref distanceHitList,
                collisionFilterHeavy))
            {
                node.cost = HEAVY_COST;
                gridCostArray[CalculateIndex(node.x, node.y, width)] = cost;
            }
            distanceHitList.Dispose();
            //entityCommandBuffer.SetComponent<GridSysterm.Node>(ID, entity, node);
        }
    }
}
