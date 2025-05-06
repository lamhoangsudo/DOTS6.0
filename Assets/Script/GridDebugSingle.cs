using UnityEngine;

public class GridDebugSingle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private int x;
    private int y;
    public void SetUp(int x, int y, float cellsize)
    {
        this.x = x;
        this.y = y;
        transform.position = GridSysterm.GetWorldPosition(x, y, cellsize);
    }
    public void UpdateColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
