using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabase objectsDatabase; // Reference to the database of objects

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData, objectData;

    [SerializeField]
    private PreviewSystem previewSystem; // Reference to the preview system

    private Vector2Int lastDetectedPosition = Vector2Int.zero; // To track the last detected position for placement

    [SerializeField]
    private ObjectPlacer objectPlacer; // Reference to the object placer to keep track of placed objects

    IBuildingState buildingState;

    [SerializeField]
    private DefaultPlacementData defaultPlacementData;
    private void Start()
    {
        StopPlacement();
        floorData = new (); 
        objectData = new();
        LoadAllObjects();
    }
    public void StartPlacement(int objectID)
    {
        StopPlacement();
        gridVisualization.SetActive(true); // Show grid visualization
        buildingState = new PlacementState(objectID,
                                           grid,
                                           previewSystem,
                                           objectsDatabase,
                                           floorData,
                                           objectData,
                                           gridVisualization,
                                           objectPlacer); // Initialize the building state
        inputManager.Onclicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, previewSystem, floorData, objectData, objectPlacer);
        inputManager.Onclicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector2Int cell2D = new Vector2Int(gridPosition.x, gridPosition.y);
        buildingState.OnAction(cell2D);
        SaveAllObjects();
    }
    //private bool CheckPlacementValidity(Vector2Int gridPosition, int selectedObjectIndex)
    //{
    //    GridData selectedData = objectsDatabase.objectsData[selectedObjectIndex].ID == 0 ? floorData : objectData; // Assuming 0 is for floor, others for object
    //    return selectedData.CanPlaceObjectAt(gridPosition, objectsDatabase.objectsData[selectedObjectIndex].Size);
    //}

    public void StopPlacement()
    {
        if (buildingState == null) { return; } // If no building state, do nothing
        gridVisualization.SetActive(false); // Show grid visualization
        buildingState?.EndState(); // End the current building state
        inputManager.Onclicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector2Int.zero; // Reset last detected position
        buildingState = null; // Clear the building state
    }


    // Update is called once per frame
    void Update()
    {
        if(buildingState == null) { return; } // If no building state, do nothing

        // Get mouse and grid positions
        Vector2 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(new Vector3(mousePosition.x, mousePosition.y, 0f));
        Vector2Int currentCell = new Vector2Int(gridPosition.x, gridPosition.y);

        // Only update when mouse moves to a new cell
        if (lastDetectedPosition != currentCell)
        {
            buildingState.UpdateState(currentCell); // Update the building state with the new cell position
            lastDetectedPosition = currentCell;
        }
    }

    public void SaveAllObjects()
    {
        List<SavedPlacementData> savedObjects = new();

        foreach (var data in floorData.GetUniquePlacements())
        {
            savedObjects.Add(new SavedPlacementData
            {
                position = data.occupiedPositions[0],
                objectID = data.ID,
                prefabIndex = data.PlacedObjectID,
                uniqueID = data.UniqueID
            });
        }

        foreach (var data in objectData.GetUniquePlacements())
        {
            savedObjects.Add(new SavedPlacementData()
            {
                position = data.occupiedPositions[0],
                objectID = data.ID,
                prefabIndex = data.PlacedObjectID,
                uniqueID = data.UniqueID
            });
        }

        string json = JsonUtility.ToJson(new Wrapper<SavedPlacementData> { list = savedObjects });
        PlayerPrefs.SetString("SavedObjects", json);
        PlayerPrefs.Save();
    }
    public void LoadAllObjects()
    {
        if (PlayerPrefs.HasKey("SavedObjects"))
        {
            string json = PlayerPrefs.GetString("SavedObjects");
            var wrapper = JsonUtility.FromJson<Wrapper<SavedPlacementData>>(json);

            foreach (var item in wrapper.list)
            {
                var objData = objectsDatabase.objectsData.Find(x => x.ID == item.objectID);
                if (objData == null) continue;

                Vector3 worldPos = grid.CellToWorld(new Vector3Int(item.position.x, item.position.y, 0));
                int index = objectPlacer.PlaceObject(objData.Prefab, worldPos, item.uniqueID, objData.Type);

                GridData selectedData = item.objectID == 0 ? floorData : objectData;
                selectedData.AddObjectAt(item.position, objData.Size, item.objectID, index, item.uniqueID);
            }
        }
        else
        {
            foreach (var item in defaultPlacementData.defaultPlacements)
            {
                var objData = objectsDatabase.objectsData.Find(x => x.ID == item.objectID);
                if (objData == null) continue;

                Vector3 worldPos = grid.CellToWorld(new Vector3Int(item.gridPosition.x, item.gridPosition.y, 0));
                string uid = Guid.NewGuid().ToString();
                int index = objectPlacer.PlaceObject(objData.Prefab, worldPos, uid, objData.Type);

                GridData selectedData = item.objectID == 0 ? floorData : objectData;
                selectedData.AddObjectAt(item.gridPosition, objData.Size, item.objectID, index, uid);
            }
            SaveAllObjects();
        }
    }

    [System.Serializable]
    public class Wrapper<T>
    {
        public List<T> list;
    }
}

[System.Serializable]
public class SavedPlacementData
{
    public Vector2Int position;
    public int objectID;
    public int prefabIndex;
    public string uniqueID;
}
