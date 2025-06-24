using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlacementData", menuName = "Scriptable Objects/Default Placement Data")]
public class DefaultPlacementData : ScriptableObject
{
    public List<DefaultPlacementItem> defaultPlacements;
}


[Serializable]
public class DefaultPlacementItem
{
    public int objectID; // selected via dropdown
    public Vector2Int gridPosition;

    // Optional utility if needed
    public ObjectData GetObjectData(ObjectsDatabase db)
    {
        return db.objectsData.Find(x => x.ID == objectID);
    }
}