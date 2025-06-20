using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector2Int, PlacementData> placedObjects = new ();
    
    public void AddObjectAt(Vector2Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector2Int> occupiedPositions = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(ID, placedObjectIndex, occupiedPositions);
        foreach (var position in occupiedPositions)
        {
            if (!placedObjects.ContainsKey(position))
            {
                placedObjects[position] = data;
            }
            else
            {
                throw new Exception($"Dictionary already contains this position: {position}");
            }
        }
    }

    private List<Vector2Int> CalculatePositions(Vector2Int gridPosition, Vector2Int objectSize)
    {
        List<Vector2Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector2Int(x,y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector2Int gridPosition, Vector2Int objectSize)
    {
        List<Vector2Int> occupiedPositions = CalculatePositions(gridPosition, objectSize);
        foreach (var position in occupiedPositions)
        {
            if (placedObjects.ContainsKey(position))
            {
                return false; // Position is already occupied
            }
        }
        return true; // All positions are free
    }
    internal int GetRepresentationIndex(Vector2Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectID;
    }
    internal void RemoveObjectAt(Vector2Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
    public List<PlacementData> GetUniquePlacements()
    {
        HashSet<PlacementData> unique = new();
        List<PlacementData> results = new();

        foreach (var entry in placedObjects.Values)
        {
            if (unique.Add(entry))
            {
                results.Add(entry);
            }
        }

        return results;
    }
}

public class PlacementData
{
    public List<Vector2Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectID { get; private set; }
    public PlacementData(int id, int placedObjectId, List<Vector2Int> occupiedPositions)
    {
        ID = id;
        PlacedObjectID = placedObjectId;
        this.occupiedPositions = occupiedPositions;
    }
    // Parameterless constructor for deserialization
    public PlacementData() { }
}


