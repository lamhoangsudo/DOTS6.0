using UnityEngine;
using UnityEngine.UI;

public class BuildingplacementManagerUI_ButtonSingle : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image outline;
    private BuildingTypeSO buildingTypeSO;
    public void Setup(BuildingTypeSO buildingTypeSO)
    {
        this.buildingTypeSO = buildingTypeSO;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            BuildingPlacementManager.buildingPlacementManager.SetActiveBuildingTypeSO(buildingTypeSO);
        });
        image.sprite = buildingTypeSO.icon;
    }
    public void HideSelect()
    {
        outline.enabled = false;
    }
    public void ShowSelect()
    {
        outline.enabled = true;
    }
}
