using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FarmingObjectData", menuName = "Scriptable Objects/FarmingObjectData")]
public class FarmingObjectData : ScriptableObject
{
    public List<FarmingObject> farmingObjects;    
    public FarmingObject GetFarmingObjectByID(int id)
    {
        return farmingObjects.Find(farmingObject => farmingObject.ID == id);
    }
}
[Serializable]
public class FarmingObject
{
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public FarmingObjectType TypeOfFarming { get; private set; }
    [field: SerializeField]
    public Sprite StarterUnitIcon { get; private set; }
    [field: SerializeField]
    public Sprite ProductIcon { get; private set; }
    [field: SerializeField]
    public Sprite FeedIcon { get; private set; }
    [field: SerializeField]
    public int MaxLevel{ get; private set; }
    [field: SerializeField]
    public int MaxProduction { get; private set; }
    [field: SerializeField]
    public int ProductionPerInterval { get; private set; }
    [field: SerializeField]
    public float ProductionIntervalMinutes { get; private set; }
    [field: SerializeField]
    public float DecayTimeAfterFullProduction { get; private set; }
    [field: SerializeField]
    public int PurchaseCost { get; private set; }
    [field: SerializeField]
    public int UnitsPerPurchase { get; private set; }
    [field: SerializeField]
    public int SellPricePerUnit { get; private set; }
    [field: SerializeField]
    public List<GameObject> LevelPrefabs { get; private set; }
}
public enum FarmingObjectType
{
    None,
    Plant,
    Animal
}
