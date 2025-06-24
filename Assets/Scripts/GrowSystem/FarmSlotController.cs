using UnityEngine;

public class FarmSlotController : MonoBehaviour
{
    public FarmSlot slot; // Reference to the FarmSlot, can be set externally
    public UIFarmSlotStatus uiFarmSlotStatus; // Reference to the UIFarmSlotStatus, can be set externally
    
    private bool startedFarming = false; // Flag to indicate if farming has started
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slot = transform.GetComponentInChildren<FarmSlot>();
        uiFarmSlotStatus = transform.GetComponentInChildren<UIFarmSlotStatus>();
        uiFarmSlotStatus.GetComponent<Canvas>().enabled = false; // Initially hide the UI status
        slot.SetFarmController(this); // Set this controller to the FarmSlot
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedFarming) return;
        UpdateUIStatusTimer();
    }
    private void OnMouseDown()
    {
        PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();
        if (placementSystem != null)
        {
            // If buildingState is not null, PlacementSystem is busy (placing or removing)
            var buildingStateField = typeof(PlacementSystem).GetField("buildingState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var buildingState = buildingStateField?.GetValue(placementSystem);
            if (buildingState != null)
            {

                return;
            }
        }
        OpenUiChoseFarmObjec();
    }
    public void StartFarming()
    {
        if (slot != null && !startedFarming)
        {
            startedFarming = true;
            UpdateUIStatusIcon(slot.GetFarmingObjectIcon()); // Update the UI icon with the farming object icon
            uiFarmSlotStatus.GetComponent<Canvas>().enabled = true; // Show the UI status when farming starts
        }
        else
        {
            Debug.LogWarning("FarmSlotController: Farming already started or slot is null.");
        }
    }
    public void OpenUiChoseFarmObjec()
    {
        if(slot == null)
        {
            Debug.LogWarning("FarmSlotController: Slot is null, cannot open UI for farming object selection.");
            return;
        }
        if (!slot.HasFarmingObject())
        {
            UIController uiController = FindObjectOfType<UIController>();
            if (uiController != null)
            {
                uiController.SetSlot(slot, (int)slot.GetFarmingObjectType());
            }
            else
            {
                Debug.LogWarning("UIController not found in the scene.");
            }
        }
        else
        {
            UIController uIController = FindObjectOfType<UIController>();
            if (uIController != null)
            {
                uIController.SetSlot(slot); // Show info UI for the existing farming object
                uIController.ShowInfoFarmSlot(); // Show the info UI for the farm slot
            }
            else
            {
                Debug.LogWarning("UIController not found in the scene.");
            }
        }
    }
    public void RemoveFarmObject()
    {
        startedFarming = false; // Reset the farming flag
        uiFarmSlotStatus.GetComponent<Canvas>().enabled = false; // Initially hide the UI status
    }
    public void UpdateUIStatusTimer()
    {
        if (uiFarmSlotStatus != null)
        {
            uiFarmSlotStatus.SetFill(slot.GetProductionTimeInterval(), slot.IsFullProduction());
        }
        else
        {
            Debug.LogWarning("UIFarmSlotStatus is not assigned or found in the scene.");
        }
    }
    public void UpdateUIStatusIcon(Sprite icon)
    {
        if (uiFarmSlotStatus != null)
        {
            uiFarmSlotStatus.SetIcon(icon);
        }
        else
        {
            Debug.LogWarning("UIFarmSlotStatus is not assigned or found in the scene.");
        }
    }
}
