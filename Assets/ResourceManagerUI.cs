using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform template;
    [SerializeField] private ResourceTypeListSO resourceTypeListSO;
    private Dictionary<ResourceTypeSO.ResourceType, ResourceManagerUI_Single> resourceManagerUISingleDictionary;
    private void Awake()
    {
        template.gameObject.SetActive(false);
    }
    private void Start()
    {
        ResourceManager.Instance.OnResourceAmountChanged += ResourceManager_OnResourceAmountChanged;
        SetUp();
        UpdateAmount();
    }

    private void ResourceManager_OnResourceAmountChanged(object sender, EventArgs e)
    {
        UpdateAmount();
    }

    private void SetUp()
    {
        resourceManagerUISingleDictionary = new Dictionary<ResourceTypeSO.ResourceType, ResourceManagerUI_Single>();
        foreach (Transform child in container)
        {
            if (child == template) continue;
            Destroy(child.gameObject);
        }
        foreach(ResourceTypeSO resourceType in resourceTypeListSO.resourceTypeList)
        {
            Transform resourceTransform = Instantiate(template, container);
            resourceTransform.gameObject.SetActive(true);
            ResourceManagerUI_Single resourceManagerUI_Single = resourceTransform.GetComponent<ResourceManagerUI_Single>();
            resourceManagerUI_Single.SetUp(resourceType);
            resourceManagerUISingleDictionary.Add(resourceType.resourceType, resourceManagerUI_Single);
        }
    }
    private void UpdateAmount()
    {
        foreach (ResourceTypeSO resourceTypeSO in resourceTypeListSO.resourceTypeList)
        {
            ResourceManagerUI_Single resourceManagerUI_Single = resourceManagerUISingleDictionary[resourceTypeSO.resourceType];
            resourceManagerUI_Single.UpdateAmount(ResourceManager.Instance.GetResourceAmount(resourceTypeSO.resourceType));
        }
    }
}
