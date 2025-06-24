using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopSystem
{
    private List<ShopItem> availableItems;
    private IShopUI shopUI;

    public ShopSystem(List<ShopItem> availableItems, IShopUI shopUI, int TypeShop)
    {
        this.availableItems = availableItems;
        this.shopUI = shopUI;
        switch (TypeShop)
        {
            case 0:
                UpdateAllItemStatesBuy();
                break;
            case 1:
                UpdateAllItemStatesSell();
                break;
            case 2:
                UpdateAllItemStateBuyWorker();
                break;
            default:
                break;
        }
    }
    public bool CanBuyItem(ShopItem item)
    {
        if(item.item!=null) 
            return PlayerInventoryManager.Instance.GetMoney() >= item.item.PurchaseCost;
        return PlayerInventoryManager.Instance.GetMoney() >= item.worker.PurchaseCost;
    }

    public void PurchaseItem(int farmingObjectID)
    {
        ShopItem item = availableItems.FirstOrDefault(x => x.item.ID == farmingObjectID);

        if (item == null)
        {
            return;
        }

        if (!PlayerInventoryManager.Instance.SpendMoney(item.item.PurchaseCost))
        {
            return;
        }

        PlayerInventoryManager.Instance.AddStarterUnits(item.item.ID, item.item.UnitsPerPurchase);
        UpdateAllItemStatesBuy();
    }
    public void SellItem(int farmingObjectID)
    {
        if( !PlayerInventoryManager.Instance.HasProduction(farmingObjectID))
        {
            return; // Cannot sell if the item is not owned
        }
        ShopItem item = availableItems.FirstOrDefault(x => x.item.ID == farmingObjectID);
        if (item == null)
        {
            return;
        }
        int ownedCount = PlayerInventoryManager.Instance.GetProductCount(item.item.ID);
        if (ownedCount <= 0)
        {
            return; // Cannot sell if no items are owned
        }
        int price = item.item.SellPricePerUnit; // Assuming SellPricePerUnit is the price per unit for selling
        PlayerInventoryManager.Instance.RemoveProduct(item.item.ID, ownedCount);
        PlayerInventoryManager.Instance.AddMoney(ownedCount*price); // Assuming SellPricePerUnit is the price per unit for selling
        UpdateAllItemStatesSell();
    }
    public void BuyWorker(int workerID)
    {
        ShopItem item = availableItems.FirstOrDefault(x => x.worker.ID == workerID);
        if (item == null) return;
        if (!PlayerInventoryManager.Instance.SpendMoney(item.worker.PurchaseCost))
        {
            return;
        }
        WorkerManager.Instance.AddWorker(workerID);
        UpdateAllItemStateBuyWorker();
    }
    private void UpdateAllItemStatesBuy()
    {
        foreach (var item in availableItems)
        {
            item.UpdateItemStateBuy(CanBuyItem(item));
        }
    }
    private void UpdateAllItemStatesSell()
    {
        foreach (var item in availableItems)
        {
            item.UpdateItemStateSell(PlayerInventoryManager.Instance.HasProduction(item.item.ID));
        }
    }
    private void UpdateAllItemStateBuyWorker()
    {
        foreach (var item in availableItems)
        {
            item.UpdateItemStateBuyWorker(CanBuyItem(item));
        }
    }
}
