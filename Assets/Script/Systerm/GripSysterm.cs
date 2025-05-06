#define GRID_DEBUG
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct GridSysterm : ISystem
{
    public struct GridSystemData : IComponentData
    {
        public int width;
        public int height;
        public float cellSize;
        public GridMap gridMap;
    }
    public struct Node : IComponentData
    {
        public int x;
        public int y;
        public byte data;
    }
    public struct GridMap
    {
        public NativeArray<Entity> gridEntityArray;
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
        GridMap gridMap = new GridMap
        {
            gridEntityArray = new NativeArray<Entity>(totalCells, Allocator.Persistent)
        };
        state.EntityManager.Instantiate(entityNode, gridMap.gridEntityArray);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = CalculateIndex(x, y, width);
                Entity entity = gridMap.gridEntityArray[index];
                state.EntityManager.SetComponentData(entity, new Node
                {
                    x = x,
                    y = y,
                });
#if GRID_DEBUG
                state.EntityManager.SetName(entity, $"Node_{x}_{y}");
#endif
                SystemAPI.SetComponent<Node>(entity, new Node
                {
                    x = x,
                    y = y,
                });
            }
        }
        state.EntityManager.AddComponent<GridSystemData>(state.SystemHandle);
        state.EntityManager.SetComponentData<GridSystemData>(state.SystemHandle, new GridSystemData
        {
            width = width,
            height = height,
            cellSize = cellSize,
            gridMap = gridMap
        });
    }
#if !GRID_DEBUG
    [BurstCompile]
#endif
    public void OnUpdate(ref SystemState state)
    {
        GridSystemData gripSystermData = SystemAPI.GetComponent<GridSystemData>(state.SystemHandle);
        if (Input.GetMouseButton(0))
        {
            float3 mouseWorldPosition = MouseWorldPositionManager.mouseWorldPositionManager.GetMousePosition();
            int2 gridPosition = GetGridPosition(mouseWorldPosition, gripSystermData.cellSize);
            if (!IsInGrid(gridPosition, gripSystermData.width, gripSystermData.height))
            {
                return;
            }
            int index = CalculateIndex(gridPosition.x, gridPosition.y, gripSystermData.width);
            Entity entity = gripSystermData.gridMap.gridEntityArray[index];
            SystemAPI.GetComponentRW<Node>(entity).ValueRW.data = 1;
        }
#if GRID_DEBUG
        GridSystermDebug.Instance?.InitializedGrid(gripSystermData);
        GridSystermDebug.Instance?.UpdateGrid(gripSystermData);
#endif
    }

#if !GRID_DEBUG
    [BurstCompile]
#endif
    public void OnDestroy(ref SystemState state)
    {
        SystemAPI.GetComponentRW<GridSystemData>(state.SystemHandle).ValueRW.gridMap.gridEntityArray.Dispose();
    }
    public static int CalculateIndex(int x, int y, int width)
    {
        return x + y * width;
    }
    public static float3 GetWorldPosition(int x, int y, float cellSize)
    {
        return new float3(x * cellSize, 0, y * cellSize);
    }
    public static int2 GetGridPosition(float3 position, float cellsize)
    {
        return new int2((int)math.floor(position.x / cellsize), (int)math.floor(position.z / cellsize));
    }
    public static bool IsInGrid(int2 gridPosition, int width, int height)
    {
        return gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height;
    }
}
