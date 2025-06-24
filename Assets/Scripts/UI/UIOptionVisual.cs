using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionVisual : MonoBehaviour
{
    [Header("UI Object building")]
    public Image Icon; // Reference to the Image component for the icon
    public TextMeshProUGUI Price; // Reference to the Text component for the price

    [Header("UI Object farming")]
    public TextMeshProUGUI _TextLVFarmSlot; // Reference to the Text component for the farming slot level
    public Image IconProduct; // Reference to the Image component for the farming icon
    public TextMeshProUGUI _TextProductAmount; // Reference to the Text component for the product amount 

    [Header("UI Farm Option")]
    public TextMeshProUGUI _TextFarmOptionAmount; // Reference to the Text component for destruction confirmation
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetDataBuilding(ObjectData data)
    {
        if (Icon != null)
            Icon.sprite = data.Icon;

        if (Price != null)
            Price.text = data.Price.ToString();

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();
                if (placementSystem != null)
                    placementSystem.StartPlacement(data.ID);
            });
        }
        else
        {
            Debug.LogWarning("Button component not found on UIOptionVisual.");
        }
    }
    public void SetDataFarm(FarmingObject data, FarmSlot slot)
    {
        if (Icon != null)
            Icon.sprite = data.StarterUnitIcon;
        if (_TextFarmOptionAmount != null)
        {
            _TextFarmOptionAmount.text = "X "+ PlayerInventoryManager.Instance.GetStarterUnitCount(data.ID).ToString(); // Assuming PlayerInventoryManager has a method GetStarterUnitCount to get the count of the farming object
        }
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                Debug.Log("Adding farming object: " + data.Name);
                slot.AddFarmObject(data); // Assuming FarmSlot has a method AddFarmObject to add the farming object
                PlayerInventoryManager.Instance.RemoveStarterUnits(data.ID, 1); // Assuming PlayerInventoryManager has a method RemoveStarterUnits to remove the farming object from inventory
                FindAnyObjectByType<UIController>().HideUI(); // Hide the UI after adding the farming object
            });
        }
        else
        {
            Debug.LogWarning("Button component not found on UIOptionVisual.");
        }
    }
    public void SetDestructionButton()
    {
        // Assuming you want to set up a button for destruction, you can add a listener here
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => 
            {
                PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();
                if (placementSystem != null)
                {
                    placementSystem.StartRemoving(); // Assuming PlacementSystem has a method StartRemoving()
                }
            });
        }
        else
        {
            Debug.LogWarning("Button component not found on UIOptionVisual.");
        }
    }

    public void SetUIFarmSlotName(string name)
    {
        if (_TextLVFarmSlot != null)
            _TextLVFarmSlot.text = name;
        else
            Debug.LogWarning("Text component for farming slot name not found.");
    }
    public void SetUIFarmSlotProduct(FarmSlot data)
    {
        if(IconProduct != null)
            IconProduct.sprite = data.GetFarmingObjectIcon();
        else
            Debug.LogWarning("Image component for farming product icon not found.");
        if (!data.NeedFeeding() && data.FarmObjectIsMaxLevel())
        {
            if (_TextProductAmount != null)
            {
                _TextProductAmount.gameObject.SetActive(true); // Show the product amount text
                _TextProductAmount.text = data.GetInfoAmountOfProduction(); // Assuming MaxProduction is the amount produced
                GetComponent<Button>().onClick.RemoveAllListeners(); // Remove any previous listeners to avoid conflicts
                GetComponent<Button>().onClick.AddListener(() =>
                {
                    data.AddGatheringTask(); // Assuming FarmSlot has a method GatherObject to collect the product
                    FindAnyObjectByType<UIController>().HideUI(); // Hide the UI after gathering the product
                });
            }
            else
                Debug.LogWarning("Text component for product amount not found.");
        }
        else
        {
            _TextProductAmount.gameObject.SetActive(false); // Hide the product amount text if feeding is needed
            GetComponent<Button>().onClick.RemoveAllListeners(); // Remove any previous listeners to avoid conflicts
            GetComponent<Button>().onClick.AddListener(() =>
            {
                data.AddFeedingTask(); // Assuming FarmSlot has a method Feed to feed the farming object
                FindAnyObjectByType<UIController>().HideUI(); // Hide the UI after feeding
            });
        }
    }
    public void SetUIDeleteFarmObject(FarmSlot data)
    {
        GetComponent<Button>().onClick.RemoveAllListeners(); // Remove any previous listeners to avoid conflicts
        GetComponent<Button>().onClick.AddListener(() =>
        {
            data.RemoveFarmObject(); // Assuming FarmSlot has a method RemoveFarmObject to remove the farming object
            FindAnyObjectByType<UIController>().HideUI(); // Hide the UI after removing the farming object
        });
    }
    public void SetCloseFarmSlotStatus()
    {
        GetComponent<Button>().onClick.RemoveAllListeners(); // Remove any previous listeners to avoid conflicts
        GetComponent<Button>().onClick.AddListener(() =>
        {
            FindAnyObjectByType<UIController>().HideUI(); // Hide the UI when the close button is clicked
        });
    }
    private void OnDestroy()
    {
        transform.DOKill(); // Kill any ongoing animations when this object is destroyed
    }
}
