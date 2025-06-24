using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUIVisual : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemOwnerText;
    public TextMeshProUGUI itemCostText;
    public TextMeshProUGUI unitsPerPurchaseText;

    public Image icon;

    public ItemUI Parent { get; set; }

    public void SetUIShopItem(FarmingObject item, int typeOfShop)
    {
        itemNameText.text = item.Name;
        int owner = typeOfShop == 0 ? PlayerInventoryManager.Instance.GetStarterUnitCount(item.ID) : PlayerInventoryManager.Instance.GetProductCount(item.ID);
        itemOwnerText.text = "Owned: " + owner;
        itemCostText.text = item.PurchaseCost.ToString();
        unitsPerPurchaseText.text = " / " + item.UnitsPerPurchase.ToString();
        icon.sprite = typeOfShop == 0 ? item.StarterUnitIcon:item.ProductIcon;
    }
    public void SetUIShopWorker(WorkerData item, int typeOfShop)
    {
        itemNameText.text = item.Name;
        int owner = FindAnyObjectByType<TaskManager>().CountWorker();
        itemOwnerText.text = "Owned: " + owner;
        itemCostText.text = item.PurchaseCost.ToString();
        unitsPerPurchaseText.text = " / 1" ;
        icon.sprite = item.IconWorker;
    }

    public void UpdateItemState(bool canBuy)
    {
        Button button = GetComponent<Button>();
        button.interactable = canBuy; // Enable or disable the button based on whether the item can be bought
    }

    public void OnClickItem(Action OnclickAction)
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => 
        {
            OnclickAction?.Invoke();
        });
    }
}
