using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManagerUI_Single : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;
    public void SetUp(ResourceTypeSO resourceTypeSO)
    {
        icon.sprite = resourceTypeSO.icon;
        amountText.text = "0";
    }
    public void UpdateAmount(int amount)
    {
        amountText.text = amount.ToString();
    }
}
