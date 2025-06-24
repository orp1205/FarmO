using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int money;

    // Dictionary to hold starter units with their farming object ID as key and amount as value
    public Dictionary<int, int> starterUnits = new Dictionary<int, int>();

    // Dictionary to hold products with their farming object ID as key and amount as value
    public Dictionary<int, int> products = new Dictionary<int, int>();

    public PlayerData()
    {
        money = 0;
        starterUnits = new Dictionary<int, int>();
        products = new Dictionary<int, int>();
    }

    public void AddStarterUnit(int farmingObjectID, int amount)
    {
        if (starterUnits.ContainsKey(farmingObjectID))
            starterUnits[farmingObjectID] += amount;
        else
            starterUnits[farmingObjectID] = amount;
    }

    public void AddProduct(int farmingObjectID, int amount)
    {
        if (products.ContainsKey(farmingObjectID))
            products[farmingObjectID] += amount;
        else
            products[farmingObjectID] = amount;
    }
    public void AddMoney(int amount)
    {
        money += amount;
    }
    public bool RemoveMoney(int amount)
    {
        if(money >= amount)
        {
            money -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }
    public int GetStarterUnitCount(int farmingObjectID)
    {
        return starterUnits.ContainsKey(farmingObjectID) ? starterUnits[farmingObjectID] : 0;
    }

    public int GetProductCount(int farmingObjectID)
    {
        return products.ContainsKey(farmingObjectID) ? products[farmingObjectID] : 0;
    }
}