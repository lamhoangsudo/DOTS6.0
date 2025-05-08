using TMPro;
using UnityEngine;

public class GridDebugSingle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro text;
    private int x;
    private int y;
    public void SetUp(int x, int y, float cellsize, string textString)
    {
        this.x = x;
        this.y = y;
        transform.position = GridSysterm.GetWorldPosition(x, y, cellsize);
        text.text = textString;
    }
    public void UpdateColor(Color color)
    {
        spriteRenderer.color = color;
    }
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
    public void SetRotation(Quaternion rotation)
    {
        spriteRenderer.transform.rotation = rotation;
    }
}
