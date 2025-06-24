using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStartingData", menuName = "Scriptable Objects/Player Starting Data")]
public class PlayerStartingData : ScriptableObject
{
    public int startingMoney;

    [Header("Starter Units (Seeds or Animals)")]
    public List<StarterUnitEntry> starterUnits;

    [Header("Products")]
    public List<ProductEntry> products;

    public int WinningGoldAmount = 1000; // Amount of gold to win the game 
}

[Serializable]
public class StarterUnitEntry
{
    public int farmingObjectID;
    public int amount;
}

[Serializable]
public class ProductEntry
{
    public int farmingObjectID;
    public int amount;
}

