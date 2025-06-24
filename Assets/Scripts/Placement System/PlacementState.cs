using System;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData floorData;
    GridData objectData;
    GameObject gridVisualization;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabase database,
                          GridData floorData,
                          GridData objectData,
                          GameObject gridVisualization,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.objectData = objectData;
        this.objectPlacer = objectPlacer;
        this.gridVisualization = gridVisualization;
        if (ID < 0 || ID >= database.objectsData.Count)
        {
            Debug.LogError("Invalid object ID selected for placement.");
            return;
        }
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        }
        else
            throw new System.Exception($"No object with ID {iD}");

    }
    public void EndState()
    {
        previewSystem.StopShowingPlacementPreview();
    }

    public void OnAction(Vector2Int gridPosition)
    {

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }
        string uniqueID = Guid.NewGuid().ToString();
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y)), uniqueID, database.objectsData[selectedObjectIndex].Type);
        PlayerInventoryManager.Instance.SpendMoney(database.objectsData[selectedObjectIndex].Price);
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            objectData;
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index, uniqueID);

        previewSystem.UpdatePositionPlacementPreview(grid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y)), false);
    }

    private bool CheckPlacementValidity(Vector2Int gridPosition, int selectedObjectIndex)
    {
        Vector3 worldPos = grid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
        if (!IsInsideGridVisualization(worldPos))
            return false;
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            objectData;
        if (PlayerInventoryManager.Instance.GetMoney() < database.objectsData[selectedObjectIndex].Price) return false;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }
    private bool IsInsideGridVisualization(Vector3 worldPos)
    {
        var spriteRenderer = gridVisualization.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No GridVisualization Found");
            return false;
        }

        return spriteRenderer.bounds.Contains(worldPos);
    }
    public void UpdateState(Vector2Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePositionPlacementPreview(grid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y)), placementValidity);
    }
}
