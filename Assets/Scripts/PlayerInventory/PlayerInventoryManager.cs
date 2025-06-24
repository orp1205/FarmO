using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance { get; private set; }

    // Player resources
    private int money;
    private Dictionary<int, int> starterUnits = new(); // key: FarmingObjectID
    private Dictionary<int, int> products = new();     // key: FarmingObjectID

    //Default player starting data
    [SerializeField]
    private PlayerStartingData defaultData;
    [SerializeField]
    private FarmingObjectData farmingObjectData; // Reference to the FarmingObjectData scriptable object
    [SerializeField]
    private TextMeshProUGUI moneyText; // Optional: UI text to display money, if needed
    [SerializeField]
    private TextMeshProUGUI progressWinningText; // Optional: UI text to display progress, if needed
    [SerializeField]
    private Slider progressWinningSlider; // Optional: UI slider to display progress, if needed
    [SerializeField]
    private TextMeshProUGUI workerCountText; // Optional: UI text to display worker count, if needed


    [SerializeField]
    private GameObject WinningScreen; // Optional: UI element to show when the player wins



    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        progressWinningSlider.maxValue = defaultData.WinningGoldAmount;
        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        LoadInventory();
    
        if(money < defaultData.WinningGoldAmount)
        {
            WinningScreen.SetActive(false); // Hide winning screen if the player hasn't won yet
        }
    }

    #region Money

    public int GetMoney() => money;

    private void UpdateMoney()
    {
        if (moneyText != null) moneyText.text = money.ToString();
        if (progressWinningText != null) progressWinningText.text = $"{money}/{defaultData.WinningGoldAmount}";
        if (progressWinningSlider != null)
        {
            progressWinningSlider.value = money;
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        if(money >= defaultData.WinningGoldAmount)
        {
            WinningScreen.SetActive(true); // Show winning screen if the player has enough money
        }
        UpdateMoney();
        SaveInventory();
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount) return false;
        money -= amount;
        UpdateMoney();
        SaveInventory();
        return true;
    }

    #endregion

    #region Starter Units (Seeds / Animals)

    public int GetStarterUnitCount(int farmingObjectID)
    {
        return starterUnits.ContainsKey(farmingObjectID) ? starterUnits[farmingObjectID] : 0;
    }

    public void AddStarterUnits(int farmingObjectID, int amount)
    {
        if (starterUnits.ContainsKey(farmingObjectID))
            starterUnits[farmingObjectID] += amount;
        else
            starterUnits[farmingObjectID] = amount;

        SaveInventory();
    }

    public bool RemoveStarterUnits(int farmingObjectID, int amount)
    {
        if (GetStarterUnitCount(farmingObjectID) < amount) return false;
        starterUnits[farmingObjectID] -= amount;
        SaveInventory();
        return true;
    }
    public List<FarmingObject> GetAllStarterUnits()
    {
        List<FarmingObject> farmingObjects = new List<FarmingObject>();
        if (starterUnits.Count == 0) return farmingObjects;
        for(int i = 0; i < farmingObjectData.farmingObjects.Count; i++)
        {
            FarmingObject farmingObject = farmingObjectData.farmingObjects[i];
            if (starterUnits.ContainsKey(farmingObject.ID) && starterUnits[farmingObject.ID] > 0)
            {
                farmingObjects.Add(farmingObject);
            }
        }
        return farmingObjects;
    }
    #endregion

    #region Products

    public int GetProductCount(int farmingObjectID)
    {
        return products.ContainsKey(farmingObjectID) ? products[farmingObjectID] : 0;
    }
    public bool HasProduction(int farmingObjectID)
    {
        return products.ContainsKey(farmingObjectID);
    }
    public void AddProduct(int farmingObjectID, int amount)
    {
        if (products.ContainsKey(farmingObjectID))
            products[farmingObjectID] += amount;
        else
            products[farmingObjectID] = amount;

        SaveInventory();
    }
    public List<FarmingObject> GetAllProduct()
    {
        List<FarmingObject> farmingObjects = new List<FarmingObject>();
        if (products.Count == 0) return farmingObjects;
        for(int i = 0; i < farmingObjectData.farmingObjects.Count; i++)
        {
            FarmingObject farmingObject = farmingObjectData.farmingObjects[i];
            if (products.ContainsKey(farmingObject.ID) && products[farmingObject.ID] > 0)
            {
                farmingObjects.Add(farmingObject);
            }
        }
        return farmingObjects;
    }
    public bool RemoveProduct(int farmingObjectID, int amount)
    {
        if (GetProductCount(farmingObjectID) < amount) return false;
        products[farmingObjectID] -= amount;
        SaveInventory();
        return true;
    }

    #endregion

    #region Save / Load

    private const string SaveKey = "PlayerInventoryData";
    [System.Serializable]
    private class IntIntPair
    {
        public int key;
        public int value;
    }
    [System.Serializable]
    private class SaveData
    {
        public int Money;
        public List<IntIntPair> starterUnits;
        public List<IntIntPair> products;
    }

    private void SaveInventory()
    {
        SaveData data = new()
        {
            Money = money,
            starterUnits = new List<IntIntPair>(),
            products = new List<IntIntPair>(),
        };

        foreach (var kvp in starterUnits)
            data.starterUnits.Add(new IntIntPair { key = kvp.Key, value = kvp.Value });

        foreach (var kvp in products)
            data.products.Add(new IntIntPair { key = kvp.Key, value = kvp.Value });

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            // Load default from ScriptableObject
            if (defaultData != null)
            {
                money = defaultData.startingMoney;
                starterUnits = new();
                products = new();

                foreach (var entry in defaultData.starterUnits)
                {
                    starterUnits[entry.farmingObjectID] = entry.amount;
                }

                foreach (var entry in defaultData.products)
                {
                    products[entry.farmingObjectID] = entry.amount;
                }
            }
            else
            {
                Debug.LogWarning("No defaultData assigned in PlayerInventoryManager.");
                money = 0;
                starterUnits = new();
                products = new();
            }
            UpdateMoney(); // Update UI if needed
            return;
        }

        // Load saved data...
        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        money = data.Money;
        starterUnits = new Dictionary<int, int>();
        products = new Dictionary<int, int>();
        UpdateMoney(); // Update UI if needed
        if (data.starterUnits != null)
            foreach (var pair in data.starterUnits)
                starterUnits[pair.key] = pair.value;

        if (data.products != null)
            foreach (var pair in data.products)
                products[pair.key] = pair.value;
    }
    #endregion
}
