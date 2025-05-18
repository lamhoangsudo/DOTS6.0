using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform buildingContainer;
    [SerializeField] private RectTransform buildingTemplate;
    [SerializeField] private BuildingTypeListSO buildingTypeListSO;
    private Dictionary<BuildingTypeSO, BuildingplacementManagerUI_ButtonSingle> buildingTypeSOToButtonSingleDictionary = new();
    private void Awake()
    {
        buildingTemplate.gameObject.SetActive(false);
        foreach (BuildingTypeSO buildingTypeSO in buildingTypeListSO.buildingTypeList)
        {
            if (!buildingTypeSO.showInBuildingPlacementUI) continue;
            RectTransform rectTransform = Instantiate(buildingTemplate, buildingContainer);
            BuildingplacementManagerUI_ButtonSingle buildingplacementManagerUI_ButtonSingle = rectTransform.GetComponent<BuildingplacementManagerUI_ButtonSingle>();
            buildingplacementManagerUI_ButtonSingle.Setup(buildingTypeSO);
            buildingTypeSOToButtonSingleDictionary.Add(buildingTypeSO, buildingplacementManagerUI_ButtonSingle);
            rectTransform.gameObject.SetActive(true);
        }
    }
    private void Start()
    {
        BuildingPlacementManager.buildingPlacementManager.OnSelectedBuildingTypeSOChanged += BuildingPlacementManager_OnSelectedBuildingTypeSOChanged;
        UpdateSelectVisual();
    }

    private void BuildingPlacementManager_OnSelectedBuildingTypeSOChanged(object sender, EventArgs e)
    {
        UpdateSelectVisual();
    }

    private void UpdateSelectVisual()
    {
        foreach(BuildingTypeSO buildingTypeSO in buildingTypeSOToButtonSingleDictionary.Keys)
        {
            buildingTypeSOToButtonSingleDictionary[buildingTypeSO].HideSelect();
        }
        buildingTypeSOToButtonSingleDictionary[BuildingPlacementManager.buildingPlacementManager.GetActiveBuildingTypeSO()].ShowSelect();
    }
}
