using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IShopUI
{
    void InitShop();
    void InitBuyShop(List<FarmingObject> itemDataBase);
    void InitSellShop(List<FarmingObject> itemDataBase);
    void InitBuyWorkerShop(List<WorkerData> itemDataBase);
    void CloseShop();
}
