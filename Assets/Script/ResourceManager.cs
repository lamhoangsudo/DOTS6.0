using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    [SerializeField] private ResourceTypeListSO resourceTypeListSO;
    public Dictionary<ResourceTypeSO.ResourceType, int> resourceTypeDictionary;
    public event EventHandler OnResourceAmountChanged;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one ResourceManager in the scene.");
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        resourceTypeDictionary = new Dictionary<ResourceTypeSO.ResourceType, int>();
        foreach(ResourceTypeSO resourceTypeSO in resourceTypeListSO.resourceTypeList)
        {
            resourceTypeDictionary[resourceTypeSO.resourceType] = 0;
        }
    }
    private void Start()
    {
        AddResourceAmount(ResourceTypeSO.ResourceType.Iron, 100);
        AddResourceAmount(ResourceTypeSO.ResourceType.Gold, 100);
        AddResourceAmount(ResourceTypeSO.ResourceType.Oil, 100);
    }
    public void AddResourceAmount(ResourceTypeSO.ResourceType resourceType, int amount)
    {
        if (resourceTypeDictionary.ContainsKey(resourceType))
        {
            resourceTypeDictionary[resourceType] += amount;
            OnResourceAmountChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Debug.LogError($"Resource type {resourceType} not found in dictionary.");
        }
    }
    public int GetResourceAmount(ResourceTypeSO.ResourceType resourceType)
    {
        return resourceTypeDictionary[resourceType];
    }
    public bool HasEnoughResource(ResourceAmount resourceAmount)
    {
        return resourceTypeDictionary[resourceAmount.resourceType] >= resourceAmount.amount;
    }
    public bool HasEnoughResource(ResourceAmount[] resourceAmounts)
    {
        foreach(ResourceAmount resourceAmount in resourceAmounts)
        {
            if (resourceTypeDictionary[resourceAmount.resourceType] < resourceAmount.amount)
            {
                return false;
            }
        }
        return true;
    }
    public void SpendResourceAmount(ResourceAmount resourceAmount)
    {
        resourceTypeDictionary[resourceAmount.resourceType] -= resourceAmount.amount;
        OnResourceAmountChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SpendResourceAmount(ResourceAmount[] resourceAmounts)
    {
        foreach (ResourceAmount resourceAmount in resourceAmounts)
        {
            resourceTypeDictionary[resourceAmount.resourceType] -= resourceAmount.amount;
        }
        OnResourceAmountChanged?.Invoke(this, EventArgs.Empty);
    }
}
