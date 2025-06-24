using UnityEngine;

public class FarmController : MonoBehaviour
{
    [SerializeField] private FarmingObjectData farmingObjectData;

    public FarmingObject GetFarmingObjectById(int id)
    {
        if (farmingObjectData == null || farmingObjectData.farmingObjects == null)
        {
            Debug.LogError("FarmingObjectData is not set or is empty.");
            return null;
        }
        foreach (var farmingObject in farmingObjectData.farmingObjects)
        {
            if (farmingObject.ID == id)
            {
                return farmingObject;
            }
        }
        Debug.LogWarning($"Farming object with ID {id} not found.");
        return null;
    }
}
