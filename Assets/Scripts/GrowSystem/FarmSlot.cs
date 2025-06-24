using System;
using UnityEngine;

public class FarmSlot : MonoBehaviour
{
    [SerializeField]
    private string SlotUID;// Unique identifier for the slot, can be used to reference this slot in other systems
    [SerializeField]
    private FarmSlotController farmController;
    public bool InQueueTask = false; // Flag to indicate if this slot is in a task queue
    [HideInInspector]
    public string slotUID
    {
        get { return SlotUID; }
        set { SlotUID = value; }
    }
    [SerializeField]
    private FarmingObjectType Type = FarmingObjectType.None; // Type of farming object this slot can hold, default is None
    [HideInInspector]
    public FarmingObjectType type
    {
        get { return Type; }
        set { Type = value; }
    }

    private FarmingObject farmingObject;
    private FarmingSlotData farmingSlotData;

    private bool needFeeding = false;

    private float productionCurrentTimer { get; set; } = 0f; // Timer to track production intervals

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateProductionTimer(Time.deltaTime);
    }
    public void SetFarmController(FarmSlotController controller)
    {
        farmController = controller; // Assign the FarmSlotController to this slot
        if (HasFarmingObject())
        {
            farmController.StartFarming(); // Start farming if a farming object is already assigned
        }
    }
    public void AddFarmObject(FarmingObject farmObject)
    {
        if (farmController != null)
        {
            farmingObject = farmObject; // Assign the farming object to the controller
            farmingSlotData = new FarmingSlotData(farmObject.ID, farmingObject.MaxLevel, farmObject.Name, farmObject.MaxProduction);
            GameObject farmingPrefab = Instantiate(farmingObject.LevelPrefabs[farmingSlotData.Level - 1], this.transform);
            needFeeding = false;
            farmController.StartFarming(); // Start farming when a farming object is added
            SaveFarmSlot(); // Save the farming slot data
        }
        else
        {
            Debug.LogWarning("FarmSlotController is not assigned. Cannot add farming object.");
        }
    }
    public void GatherObject()
    {
        if (farmingSlotData != null && farmingSlotData.HasProduction())
        {
            // Logic to gather the produced items
            int gatheredAmount = farmingSlotData.CurrentProduction;

            // Here you can add the gathered items to the player's inventory or perform any other action
            PlayerInventoryManager.Instance.AddProduct(farmingObject.ID, gatheredAmount); // Add the gathered items to the player's inventory
            // Reset production after gathering
            if (farmingSlotData.ResetProduction())
            {
                RemoveFarmObject(); // Remove the farming object if production is reset
            }
        }
        else
        {
            Debug.Log("No items to gather.");
        }
        InQueueTask = false; // Reset the task queue flag after gathering
        SaveFarmSlot(); // Save the farming slot data after gathering
    }
    public void RemoveFarmObject()
    {
        // Logic to remove the farming object from the slot
        if (farmingSlotData != null)
        {
            farmingSlotData = null; // Clear the slot data
            farmingObject = null; // Clear the farming object reference
            productionCurrentTimer = 0f; // Reset the production timer
            farmController.RemoveFarmObject(); // Notify the FarmSlotController to remove this slot
            DeleteSaveFarmSlot(); // Delete the saved data for this slot
            Destroy(this.transform.GetChild(0).gameObject);
        }
    }
    private void UpdateProductionTimer(float deltaTime)
    {
        if (farmingObject == null)
        {
            return; // If no farming object is assigned, do nothing
        }

        if (needFeeding) return; // If the farming object needs feeding, skip production update
        productionCurrentTimer += deltaTime; // Increment the production timer
        if (farmingSlotData.IsMaxLevel())
        {
            if (farmingSlotData.IsFullProduction())
            {
                if (productionCurrentTimer >= farmingObject.DecayTimeAfterFullProduction * 60f) // Convert minutes to seconds
                {
                    RemoveFarmObject(); // Produce items if full production time has passed
                    productionCurrentTimer = 0f; // Reset the timer after production
                }
            }
            else
            {
                if (productionCurrentTimer >= farmingObject.ProductionIntervalMinutes * 60f) // Convert minutes to seconds
                {
                    Produce(); // Produce items if not full production time has passed
                    productionCurrentTimer = 0f; // Reset the timer after production
                }
            }
        }
        else
        {
            if (productionCurrentTimer >= farmingObject.ProductionIntervalMinutes * 60f) // Convert minutes to seconds
            {
                if (!needFeeding)
                {
                    ///Update new object Level
                    needFeeding = true; // Set needFeeding to true if not already set
                    productionCurrentTimer = 0f; // Reset the production timer
                    UpdateFarmVisualByLevel();
                    farmController.UpdateUIStatusIcon(farmingObject.FeedIcon); // Update the UI icon to show feeding is needed
                }
            }
        }
    }
    private void UpdateFarmVisualByLevel()
    {
        if (transform.childCount > 0 && farmingObject != null)
        {
            Destroy(transform.GetChild(0).gameObject); // Destroy the current visual prefab
        }
        farmingSlotData.UpgradeLevel(); // Upgrade the farming slot level
        GameObject farmingPrefab = Instantiate(farmingObject.LevelPrefabs[farmingSlotData.Level - 1],
                                               this.transform); // Instantiate the new visual prefab based on the current level
        SaveFarmSlot(); // Save the farming slot data after LevelUp
    }
    public void Feed()
    {
        if (needFeeding)
        {
            if (farmingSlotData.IsMaxLevel())
            {
                farmController.UpdateUIStatusIcon(farmingObject.ProductIcon); // Update the UI icon to show feeding is done
            }
            else
            {
                farmController.UpdateUIStatusIcon(farmingObject.StarterUnitIcon); // Update the UI icon to show level up
            }
            needFeeding = false; // Reset needFeeding when fed
        }
        InQueueTask = false; // Reset the task queue flag when feeding is done
        SaveFarmSlot(); // Save the farming slot data after feeding
    }
    public void AddFeedingTask()
    {
        if(!NeedFeeding()) return; // If the farming object does not need feeding, do not add a task
        FindAnyObjectByType<TaskManager>().AddTask(SlotUID, transform.position, 1); // Add a feeding task to the task manager
        InQueueTask = true; // Set the slot as in queue for a task
    }
    public void AddGatheringTask()
    {
        if(farmingObject == null || farmingSlotData == null || !farmingSlotData.HasProduction())
        {
            return; // If no farming object or slot data, do not add a gathering task
        }
        FindAnyObjectByType<TaskManager>().AddTask(SlotUID, transform.position, 2); // Add a gathering task to the task manager
        InQueueTask = true; // Set the slot as in queue for a task
    }
    public string GetSlotFarmObjectName()
    {
        if (farmingObject != null)
        {
            return farmingObject.Name; // Return the name of the farming object
        }
        return "No Farming Object"; // Return a default message if no farming object is assigned
    }
    public bool FarmObjectIsMaxLevel()
    {
        return farmingSlotData.IsMaxLevel(); // Check if the farming slot is at max level
    }
    public bool NeedFeeding()
    {
        return needFeeding; // Return whether the farming object needs feeding
    }
    public bool HasFarmingObject()
    {
        return farmingObject != null; // Check if there is a farming object assigned
    }
    public bool IsFullProduction()
    {
        return farmingSlotData != null && farmingSlotData.IsFullProduction(); // Check if the farming slot is in full production
    }
    public FarmingObjectType GetFarmingObjectType()
    {
        return type; // Return the type of the farming object
    }
    public float GetProductionTimeInterval()
    {
        if (farmingObject == null) return 0f; // If no farming object is assigned, return 0
        if (farmingSlotData.IsMaxLevel() && farmingSlotData.IsFullProduction()) 
        {
            return Mathf.InverseLerp(0, farmingObject.DecayTimeAfterFullProduction * 60f, productionCurrentTimer); // Return decay time in seconds if full production
        }
        return Mathf.InverseLerp(0, farmingObject.ProductionIntervalMinutes * 60f, productionCurrentTimer); // Return production interval in seconds
    }
    private void Produce()
    {
        if (farmingSlotData == null) return; // If no farming slot data, do nothing

        farmingSlotData.AddProduction(farmingObject.ProductionPerInterval); // Add production based on the farming object's production rate
    }
    public Sprite GetFarmingObjectIcon()
    {
        if (farmingObject != null)
        {
            if (farmingSlotData.IsMaxLevel() && !NeedFeeding())
            {
                return farmingObject.ProductIcon; // Return the product icon if the farming object is at max level
            }
            return farmingObject.FeedIcon; // Return the feed icon if the farming object needs feeding
        }
        return null; // Return null if no farming object is assigned
    }
    public string GetInfoAmountOfProduction()
    {
        if (farmingSlotData != null)
        {
            return $"{farmingSlotData.CurrentProduction}/{farmingSlotData.MaxProduction}"; // Return the current and max production amounts
        }
        return "0/0"; // Return default value if no farming slot data
    }
    public bool CompareUID(string uid)
    {
        return SlotUID == uid; // Compare the slot's unique identifier with the provided UID
    }

    public void SaveFarmSlot()
    {
        PlayerPrefs.SetInt($"{slotUID}_FarmingObjectID", farmingObject.ID);
        PlayerPrefs.SetInt($"{slotUID}_Level", farmingSlotData.Level);
        PlayerPrefs.SetInt($"{slotUID}_CurrentProduction", farmingSlotData.CurrentProduction);
        PlayerPrefs.SetInt($"{slotUID}_MaxProduction", farmingSlotData.MaxProduction);
        PlayerPrefs.SetInt($"{slotUID}_NeedFeeding", needFeeding ? 1 : 0);
        PlayerPrefs.SetString($"{slotUID}_LastHarvestTime", farmingSlotData.LastHarvestTime.ToString("o")); // Save the last harvest time in ISO 8601 format
        PlayerPrefs.Save(); // Save the PlayerPrefs to persist the data
        Debug.Log("PlayerPrefs Saved!");
    }
    public void LoadFarmSlot()
    {
        if (PlayerPrefs.HasKey($"{slotUID}_FarmingObjectID"))
        {
            int farmingObjectID = PlayerPrefs.GetInt($"{slotUID}_FarmingObjectID");
            FarmingObjectData farmingObjectData = Resources.Load<FarmingObjectData>("ScriptableObject/Farming/FarmingObjectData"); // Load the FarmingObjectData from Resources
            farmingObject = farmingObjectData.farmingObjects.Find(obj => obj.ID == farmingObjectID); // Find the farming object by ID
            if (farmingObject != null)
            {
                int level = PlayerPrefs.GetInt($"{slotUID}_Level", 1);
                int currentProduction = PlayerPrefs.GetInt($"{slotUID}_CurrentProduction", 0);
                int maxProduction = PlayerPrefs.GetInt($"{slotUID}_MaxProduction");
                string lastHarvestTimeString = PlayerPrefs.GetString($"{slotUID}_LastHarvestTime", DateTime.Now.ToString("o")); // Default to current time if not set
                needFeeding = PlayerPrefs.GetInt($"{slotUID}_NeedFeeding", 0) == 1; // Convert stored integer to boolean
                if(maxProduction <= 0) // If max production is not set, use the farming object's max production
                {
                    farmingObject = null; // Set farming object to null if max production is not set
                    DeleteSaveFarmSlot(); // Delete the saved data for this slot
                    return; // Exit if max production is not set
                }
                if (level == farmingObject.MaxLevel && !needFeeding)
                {
                    if(DateTime.Now > DateTime.Parse(lastHarvestTimeString))
                    {
                        float idleTime = (float)DateTime.Now.Subtract(DateTime.Parse(lastHarvestTimeString)).TotalMinutes; // Calculate idle time since last harvest
                        int idleItem = Mathf.FloorToInt(idleTime/farmingObject.ProductionPerInterval); // Calculate the production to add based on idle time
                        if (maxProduction < currentProduction+ idleItem)
                        {
                            int productionToAdd = maxProduction - currentProduction; // Calculate how much production can be added without exceeding max production
                            float productionTime = (float)productionToAdd * farmingObject.ProductionIntervalMinutes; // Calculate the production time based on the production rate
                            float decayTime = idleTime - productionTime; // Calculate the decay time after production
                            if(decayTime < farmingObject.DecayTimeAfterFullProduction)
                            {
                                productionCurrentTimer = decayTime * 60f; // Convert decay time to seconds
                            }
                            else
                            {
                                farmingObject = null; // Set farming object to null if decay time exceeds the allowed limit
                                DeleteSaveFarmSlot(); // Delete the saved data for this slot
                                return; // Exit if decay time exceeds the allowed limit
                            }
                            idleItem = productionToAdd; // Set idle item to the production that can be added
                        }
                        float addingTime = 0;
                        addingTime = idleTime - idleItem * farmingObject.ProductionIntervalMinutes; // Calculate the time to add based on idle items
                        productionCurrentTimer = addingTime; // Set the production timer to the calculated time
                        currentProduction += idleItem; // Add the idle production to the current production
                    }
                }

                farmingSlotData = new FarmingSlotData(farmingObjectID, level, farmingObject.MaxLevel, currentProduction, maxProduction, lastHarvestTimeString);
                
                GameObject farmingPrefab = Instantiate(farmingObject.LevelPrefabs[farmingSlotData.Level - 1], this.transform); // Instantiate the farming prefab based on the level
            }
        }
    }
    public void DeleteSaveFarmSlot()
    {
        PlayerPrefs.DeleteKey($"{slotUID}_FarmingObjectID");
        PlayerPrefs.DeleteKey($"{slotUID}_Level");
        PlayerPrefs.DeleteKey($"{slotUID}_CurrentProduction");
        PlayerPrefs.DeleteKey($"{slotUID}_MaxProduction");
        PlayerPrefs.DeleteKey($"{slotUID}_NeedFeeding");
        PlayerPrefs.DeleteKey($"{slotUID}_LastHarvestTime");
        PlayerPrefs.Save(); // Save the PlayerPrefs to persist the deletion
        Debug.Log("PlayerPrefs Saved!");
    }
}
public class FarmingSlotData
{
    public int ID { get; private set; }
    string Name;
    public int MaxLevel { get; private set; }
    public int Level { get; private set; }
    public int CurrentProduction { get; private set; }
    public int MaxProduction { get; private set; }
    public DateTime LastHarvestTime { get; private set; }
    public FarmingSlotData(int id, int maxLevel, string name, int maxProduction)
    {
        this.ID = id;
        this.Name = name;
        this.MaxLevel = maxLevel;
        this.Level = 1;
        MaxProduction = maxProduction;
        this.CurrentProduction = 0;
        this.LastHarvestTime = DateTime.Now; // Initialize last harvest time to current time
    }
    public FarmingSlotData(int id, int level, int maxLevel, int currentProduction, int maxProduction, string lastHarvestTime)
    {
        this.ID = id;
        this.Level = level;
        this.MaxLevel = maxLevel;
        this.CurrentProduction = currentProduction;
        this.MaxProduction = maxProduction;
        this.LastHarvestTime = DateTime.Parse(lastHarvestTime); // Parse the last harvest time from string
    }
    public void UpgradeLevel()
    {
        Level++;
    }
    public void AddProduction(int amount)
    {
        CurrentProduction += amount;
        if (CurrentProduction > MaxProduction)
        {
            CurrentProduction = MaxProduction; // Cap production at max level
        }
    }
    public bool ResetProduction()
    {
        MaxProduction -= CurrentProduction; // Reduce max production by current production amount
        CurrentProduction = 0;
        return MaxProduction <= 0; // Return true if max production is zero or less
    }
    public bool IsMaxLevel()
    {
        return Level >= MaxLevel;
    }
    public bool HasProduction()
    {
        return CurrentProduction > 0;
    }
    public bool IsFullProduction()
    {
        return CurrentProduction >= MaxProduction;
    }
}

