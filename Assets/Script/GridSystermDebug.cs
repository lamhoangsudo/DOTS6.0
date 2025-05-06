using Unity.Entities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridSystermDebug : MonoBehaviour
{
    [SerializeField] private Transform visual;
    private GridDebugSingle[,] gridDebugSingles;
    public static GridSystermDebug Instance { get; private set; }
    private bool isDebug = false;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void InitializedGrid(GridSysterm.GridSystemData gridSystemData)
    {
        if (isDebug) return;
        gridDebugSingles = new GridDebugSingle[gridSystemData.width, gridSystemData.height];
        for (int x = 0; x < gridSystemData.width; x++)
        {
            for (int y = 0; y < gridSystemData.height; y++)
            {
                Transform transform = Instantiate(visual);
                gridDebugSingles[x, y] = transform.GetComponent<GridDebugSingle>();
                gridDebugSingles[x, y].SetUp(x, y, gridSystemData.cellSize);
            }
        }
        isDebug = true;
    }
    public void UpdateGrid(GridSysterm.GridSystemData gridSystemData)
    {
        for (int x = 0; x < gridSystemData.width; x++)
        {
            for (int y = 0; y < gridSystemData.height; y++)
            {
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                int index = GridSysterm.CalculateIndex(x, y, gridSystemData.width);
                GridSysterm.Node node = entityManager.GetComponentData<GridSysterm.Node>(gridSystemData.gridMap.gridEntityArray[index]);
                gridDebugSingles[x, y].UpdateColor(node.data == 0 ? Color.white : Color.red);
            }
        }
    }
}
