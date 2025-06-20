using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new(); // To keep track of placed objects

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject placed = Instantiate(prefab);
        placed.transform.position = new Vector3(position.x, position.y, 0f);
        placedGameObjects.Add(placed);
        return placedGameObjects.Count - 1; // Return the index of the placed object
    }
    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex
            || placedGameObjects[gameObjectIndex] == null)
            return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
