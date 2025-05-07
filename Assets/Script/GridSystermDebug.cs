using Unity.Entities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridSystermDebug : MonoBehaviour
{
    [SerializeField] private Transform visual;
    private GridDebugSingle[,] gridDebugSingles;
    [SerializeField] private Sprite circleSprite, arrowSprite;
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
                int gridIndex = gridSystemData.nextGridIndex - 1;
                if (gridIndex < 0) gridIndex = 0;
                GridSysterm.Node node = entityManager.GetComponentData<GridSysterm.Node>(gridSystemData.gridMaps[gridIndex].gridEntityArray[index]);
                if (node.cost == 0 && node.bestCost == 0)
                {
                    gridDebugSingles[x, y].SetSprite(circleSprite);
                    gridDebugSingles[x, y].UpdateColor(Color.green);
                }
                else
                {
                    if(node.cost == byte.MaxValue)
                    {
                        gridDebugSingles[x, y].SetSprite(circleSprite);
                        gridDebugSingles[x, y].UpdateColor(Color.black);
                        continue;
                    }
                    gridDebugSingles[x, y].SetSprite(arrowSprite);
                    gridDebugSingles[x, y].UpdateColor(Color.red);
                    gridDebugSingles[x, y].SetRotation(Quaternion.Euler(90, 0, Mathf.Atan2(node.vector.y, node.vector.x) * Mathf.Rad2Deg));
                }
                //gridDebugSingles[x, y].UpdateColor(node.data == 0 ? Color.white : Color.red);
            }
        }
    }
}
