using UnityEngine;

public class SelectManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform selectAreaRectTransform;
    [SerializeField] private Canvas canvas; 
    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectAreaStart += Instance_OnSelectAreaStart;
        UnitSelectionManager.Instance.OnSelectAreaEnd += Instance_OnSelectAreaEnd;
        selectAreaRectTransform.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(selectAreaRectTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }

    private void Instance_OnSelectAreaEnd(object sender, System.EventArgs e)
    {
        selectAreaRectTransform.gameObject.SetActive(false);
    }

    private void Instance_OnSelectAreaStart(object sender, System.EventArgs e)
    {
        selectAreaRectTransform.gameObject.SetActive(true);
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectAreaRect();
        float canvasScale = canvas.transform.localScale.x;
        selectAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScale;
        selectAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScale;
    }
}
