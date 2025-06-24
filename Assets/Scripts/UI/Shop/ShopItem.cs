using System;
using UnityEngine;

public interface ShopItem
{
    FarmingObject item { get; set; }
    WorkerData worker {  get; set; }
    void UpdateItemStateSell(bool canSell);
    void UpdateItemStateBuy(bool canBuy);
    void UpdateItemStateBuyWorker(bool canBuy);
    void SetUIShopItem(int TypeOfShop);
    void OnClickItem(Action onclickAction);
}
