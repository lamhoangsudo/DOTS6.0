using UnityEngine;

[CreateAssetMenu(fileName = "ResourceTypeSO", menuName = "Scriptable Objects/ResourceTypeSO")]
public class ResourceTypeSO : ScriptableObject
{
    public enum ResourceType
    {
        None = 0,
        Iron = 1,
        Gold = 2,
        Oil = 3,
    }
    public ResourceType resourceType;
    public Sprite icon;
}
