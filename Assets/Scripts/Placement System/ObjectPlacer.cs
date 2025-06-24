using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new(); // To keep track of placed objects
    public Transform ObjectContainer; // Parent container for placed objects

    public int PlaceObject(GameObject prefab, Vector3 position, string UID, FarmingObjectType type)
    {
        GameObject placed = Instantiate(prefab, ObjectContainer);
        placed.transform.position = new Vector3(position.x, position.y, 0f);
        placed.transform.GetChild(0).AddComponent<FarmSlotController>(); // Add FarmSlotController to the first child of the placed object
        FarmSlot farmSlot = placed.GetComponentInChildren<FarmSlot>();

        if (farmSlot != null)
        {
            farmSlot.slotUID = UID;
            farmSlot.type = type;
            farmSlot.LoadFarmSlot(); // Load the farm slot data if available
        }
        else
        {
            Debug.LogError($"FarmSlot component not found in prefab: {prefab.name}");
        }
        placedGameObjects.Add(placed);
        return placedGameObjects.Count - 1; // Return the index of the placed object
    }
    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex
            || placedGameObjects[gameObjectIndex] == null)
            return;
        placedGameObjects[gameObjectIndex].transform.GetChild(0).GetComponentInChildren<FarmSlot>().DeleteSaveFarmSlot(); // Stop farming if applicable
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
