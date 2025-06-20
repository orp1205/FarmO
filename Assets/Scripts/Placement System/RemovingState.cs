using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData objectData;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData objectData,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.objectData = objectData;
        this.objectPlacer = objectPlacer;
        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPlacementPreview();
    }

    public void OnAction(Vector2Int gridPosition)
    {
        GridData selectedData = null;
        if (objectData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = objectData;
        }
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }

        if (selectedData == null)
        {
            //null means no object is placed at this position
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
                return;
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cellPosition = grid.CellToWorld(new Vector3Int(gridPosition.x,gridPosition.y));
        previewSystem.UpdatePositionPlacementPreview(cellPosition, CheckIfSelectionIsValid(gridPosition));
        
    }

    private bool CheckIfSelectionIsValid(Vector2Int gridPosition)
    {
        return !(objectData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
            floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector2Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePositionPlacementPreview(grid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y)), validity);
    }
}
