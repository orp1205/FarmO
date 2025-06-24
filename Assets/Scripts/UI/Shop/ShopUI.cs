using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour, IShopUI
{
    public FarmingObjectData farmingObjectData; // Reference to the FarmingObjectData ScriptableObject
    public WorkerDatabase WorkerDatabase;
    public Transform contentPanel;
    public Transform startPosition; // Panel to hold the item UI prefabs
    public GameObject itemUIPrefab;
    public GameObject itemUIVisualPrefab; // Prefab for the item UI visuals

    public GameObject Container;

    private ShopSystem shopSystem;
    private int currentShopType = 0; // 0 for Buy FarmObject, 1 for Sell Product, 2 for Buy Worker 

    public void CloseShop()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject); // Clear existing items
        }
        shopSystem = null; // Reset the shop system
        Container.SetActive(false);
    }
    public void ChangeShopType(int type)
    {
        if (currentShopType == type) return;
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject); // Clear existing items
        }
        currentShopType = type; // Update the current shop type
        InitShop(); // Reinitialize the shop with the new type
    }
    public void InitBuyShop(List<FarmingObject> itemDataBase)
    {
        Container.SetActive(true);
        List<ShopItem> shopItems = new List<ShopItem>();
        foreach (var item in itemDataBase)
        {
            GameObject itemGameObject = Instantiate(itemUIPrefab, contentPanel);
            GameObject visualItem = Instantiate(itemUIVisualPrefab, startPosition); // Instantiate the visual prefab
            ItemUI shopItem = itemGameObject.GetComponent<ItemUI>();
            if (shopItem != null)
            {
                shopItem.item = item; // Assuming the prefab has a component to set the item data
                shopItem.ItemUIVisual = visualItem.GetComponent<ItemUIVisual>(); // Set the visual component
                shopItem.SetUIShopItem(currentShopType); // Set the UI data for the item
                shopItem.OnClickItem(() => shopSystem.PurchaseItem(item.ID)); // Add click listener
                shopItems.Add(shopItem);
            }
        }
        currentShopType = 0; // Set current shop type to Buy
        shopSystem = new ShopSystem(shopItems, this, currentShopType);
    }

    public void InitSellShop(List<FarmingObject> itemDataBase)
    {
        Container.SetActive(true);
        List<ShopItem> shopItems = new List<ShopItem>();
        foreach (var item in itemDataBase)
        {
            GameObject itemGameObject = Instantiate(itemUIPrefab, contentPanel);
            GameObject visualItem = Instantiate(itemUIVisualPrefab, startPosition); // Instantiate the visual prefab
            ItemUI shopItem = itemGameObject.GetComponent<ItemUI>();
            if (shopItem != null)
            {
                shopItem.item = item; // Assuming the prefab has a component to set the item data
                shopItem.ItemUIVisual = visualItem.GetComponent<ItemUIVisual>(); // Set the visual component
                shopItem.SetUIShopItem(currentShopType); // Set the UI data for the item
                shopItem.OnClickItem(() => shopSystem.SellItem(item.ID)); // Add click listener
                shopItems.Add(shopItem);
            }
        }
        currentShopType = 1; // Set current shop type to Sell
        shopSystem = new ShopSystem(shopItems, this, currentShopType);
    }
    public void InitBuyWorkerShop(List<WorkerData> itemDataBase)
    {
        Container.SetActive(true);
        List<ShopItem> shopItems = new List<ShopItem>();
        foreach (var item in itemDataBase)
        {
            GameObject itemGameObject = Instantiate(itemUIPrefab, contentPanel);
            GameObject visualItem = Instantiate(itemUIVisualPrefab, startPosition); // Instantiate the visual prefab
            ItemUI shopItem = itemGameObject.GetComponent<ItemUI>();
            if (shopItem != null)
            {
                shopItem.worker = item; // Assuming the prefab has a component to set the item data
                shopItem.ItemUIVisual = visualItem.GetComponent<ItemUIVisual>(); // Set the visual component
                shopItem.SetUIShopItem(currentShopType); // Set the UI data for the item
                shopItem.OnClickItem(() => shopSystem.BuyWorker(item.ID)); // Add click listener
                shopItems.Add(shopItem);
            }
        }
        currentShopType = 2; // Set current shop type to Sell
        shopSystem = new ShopSystem(shopItems, this, currentShopType);
    }

    public void InitShop()
    {
        switch(currentShopType)
        {
            case 0:
                InitBuyShop(farmingObjectData.farmingObjects); // Initialize Buy shop with items
                break;
            case 1:
                InitSellShop(PlayerInventoryManager.Instance.GetAllProduct()); // Initialize Sell shop with items
                break;
            case 2:
                InitBuyWorkerShop(WorkerDatabase.workerDataList);
                break;
            default:
                Debug.LogWarning("Invalid shop type selected.");
                break;
        }
    }
}
