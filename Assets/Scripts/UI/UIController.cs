using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Transform UIOptionZone; // The parent transform for UI options
    public Transform UIVisual; // The parent transform for the visual elements

    public GameObject UIOptionPrefab; // The prefab for UI options
    public GameObject UIOptionVisualPrefabs; // Reference to the prefab of the visual options
    public GameObject UIFarmOptionVisualPrefabs; // Reference to the prefab of the farm visual options

    public GameObject DestructionUI; // Reference to the destruction UI, if needed

    public GameObject InfoFarmSlot; // Reference to the info UI for farm slots
    public GameObject StatusFarmSlot; // Reference to the status UI for farm slots
    public GameObject DeleteFarmObject; // Reference to the delete UI for farm objects
    public GameObject CloseFarmSlotStatusUI; // Reference to the close button for farm slot status UI


    public Transform StartPosition; // The starting position for UI options

    [SerializeField] private ObjectsDatabase objectsDatabase; // Reference to the ObjectsDatabase scriptable object

    [SerializeField] private FarmingObjectData farmingObjectData; // Reference to the FarmingObjectData scriptable object

    private FarmSlot slot; // Reference to the FarmSlot, can be set externally

    public UIType CurrentUIType = UIType.None; // The current UI type being displayed

    public ShopUI shop;
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleUI(int indexUI)
    {
        UIType type = (UIType)indexUI; // Convert the index to UIType enum
        if (CurrentUIType == type)
        {
            // If the current UI type is the same as the requested type, hide it
            HideUI();
        }
        else
        {
            // Otherwise, show the requested UI type
            ShowUI(type);
        }
    }

    public void HideUI()
    {
        foreach (Transform child in UIOptionZone)
        {
            Destroy(child.gameObject); // Destroy all UI options
        }
        foreach (Transform child in UIVisual)
        {
            child.DOMove(StartPosition.position, 0.5f).SetEase(Ease.OutCubic) // Move visuals back to the start position
                .OnComplete(() => Destroy(child.gameObject)); // Destroy the visual after moving
        }
        PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();
        if (placementSystem != null)
        {
            placementSystem.StopPlacement(); // End any ongoing placement in the PlacementSystem
        }
        if(shop != null)
        {
            shop.CloseShop(); // Close the shop if it exists
        }
        CurrentUIType = UIType.None; // Reset the current UI type
    }

    private void ShowUI(UIType type)
    {
        HideUI(); // Hide any existing UI first
        CurrentUIType = type; // Set the current UI type
        switch(type)
        {
            case UIType.OptionBuilding:
                ShowOptionBuilding(); // Show building options
                break;
            case UIType.OptionPlant:
                ShowOptionPlant(); // Uncomment and implement this method for plant options
                break;
            case UIType.OptionAnimal:
                ShowOptionAnimal(); // Uncomment and implement this method for animal options
                break;
            case UIType.Shop:
                shop.InitShop(); // Initialize the shop with farming objects
                break;
            default:
                Debug.LogWarning("Unknown UI type: " + type);
                break;
        }
    }

    public void ShowOptionBuilding()
    {
        foreach(var item in objectsDatabase.objectsData)
        {
            if(item.ID <= 0) continue; // Skip invalid IDs
            GameObject option = Instantiate(UIOptionPrefab, UIOptionZone);
            GameObject visual = Instantiate(UIOptionVisualPrefabs, UIVisual);

            option.GetComponent<UIOption>().visual = visual.GetComponent<UIOptionVisual>(); // Assuming UIOption has a visual component to set data
            option.GetComponent<UIOption>().SetDataBuilding(item); // Assuming UIOption has a method to set data

            visual.transform.position = StartPosition.position; // Set the position of the visual element
        }
        var Des = Instantiate(UIOptionPrefab, UIOptionZone);
        var visualDes = Instantiate(DestructionUI, UIVisual);
        Des.GetComponent<UIOption>().visual = visualDes.GetComponent<UIOptionVisual>(); // Assuming UIOption has a visual component to set data
        Des.GetComponent<UIOption>().SetDestructionButton(); // Assuming UIOption has a method to set destruction button
        visualDes.transform.position = StartPosition.position; // Set the position of the visual element
    }

    public void ShowOptionPlant()
    {
        foreach (var item in PlayerInventoryManager.Instance.GetAllStarterUnits())
        {
            if (item.TypeOfFarming != FarmingObjectType.Plant) continue; // Skip invalid IDs
            GameObject option = Instantiate(UIOptionPrefab, UIOptionZone);
            GameObject visual = Instantiate(UIFarmOptionVisualPrefabs, UIVisual);
            option.GetComponent<UIOption>().visual = visual.GetComponent<UIOptionVisual>(); // Assuming UIOption has a visual component to set data
            option.GetComponent<UIOption>().SetDataFarm(item,slot); // Assuming UIOption has a method to set data
            visual.transform.position = StartPosition.position; // Set the position of the visual element
        }
    }
    public void ShowOptionAnimal()
    {
        foreach (var item in PlayerInventoryManager.Instance.GetAllStarterUnits())
        {
            if (item.TypeOfFarming != FarmingObjectType.Animal) continue; // Skip invalid IDs
            GameObject option = Instantiate(UIOptionPrefab, UIOptionZone);
            GameObject visual = Instantiate(UIFarmOptionVisualPrefabs, UIVisual);
            option.GetComponent<UIOption>().visual = visual.GetComponent<UIOptionVisual>(); // Assuming UIOption has a visual component to set data
            option.GetComponent<UIOption>().SetDataFarm(item, slot); // Assuming UIOption has a method to set data
            visual.transform.position = StartPosition.position; // Set the position of the visual element
        }
    }
    public void ShowInfoFarmSlot()
    {
        if(slot == null)
        {
            Debug.LogWarning("FarmSlot is not set. Cannot show info UI.");
            return;
        }
        else
        {
            GameObject infoUIName = Instantiate(UIOptionPrefab, UIOptionZone);
            GameObject infoUIStatus = Instantiate(UIOptionPrefab, UIOptionZone);
            GameObject infoUIDelete = Instantiate(UIOptionPrefab, UIOptionZone);
            GameObject infoUIClose = Instantiate(UIOptionPrefab, UIOptionZone);

            GameObject infoNameVisual = Instantiate(InfoFarmSlot, UIVisual);
            GameObject infoStatusVisual = Instantiate(StatusFarmSlot, UIVisual);
            GameObject infoDeleteVisual = Instantiate(DeleteFarmObject, UIVisual);
            GameObject infoCloseVisual = Instantiate(CloseFarmSlotStatusUI, UIVisual);

            infoUIName.GetComponent<UIOption>().visual = infoNameVisual.GetComponent<UIOptionVisual>(); // Assuming UIOption has a visual component to set data
            infoUIName.GetComponent<UIOption>().SetInfoName(slot.GetSlotFarmObjectName()); // Assuming UIOption has a method to set data for farming object

            infoUIStatus.GetComponent<UIOption>().visual = infoStatusVisual.GetComponent<UIOptionVisual>(); // Assuming UIOption has a visual component to set data
            infoUIStatus.GetComponent<UIOption>().SetUIFarmSlotProduct(slot); // Assuming UIOptionVisual has a method to set farm slot data

            infoUIDelete.GetComponent<UIOption>().visual = infoDeleteVisual.GetComponent<UIOptionVisual>(); // Assuming UIOption has a visual component to set data
            infoUIDelete.GetComponent<UIOption>().SetUIDeleteFarmObject(slot); // Assuming UIOption has a method to set delete button for farming object

            infoUIClose.GetComponent<UIOption>().visual = infoCloseVisual.GetComponent<UIOptionVisual>(); // Assuming UIOption has a visual component to set data
            infoUIClose.GetComponent<UIOption>().SetCloseFarmSlotStatus(); // Assuming UIOption has a method to set close button for farm slot status
        }
    }
    public void SetSlot(FarmSlot newSlot)
    {
        slot = newSlot; // Set the slot reference
        HideUI(); // Hide any existing UI first
    }
    public void SetSlot(FarmSlot newSlot, int typeFarm)
    {
        slot = newSlot; // Set the slot reference
        Debug.Log("Setting slot with type: " + slot.name);
        HideUI(); // Hide any existing UI first
        ToggleUI(typeFarm); // Toggle the UI based on the type of farming object
    }
}

public enum UIType
{
    None,
    OptionPlant,
    OptionAnimal,
    OptionBuilding,
    Shop,
}
