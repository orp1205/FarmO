﻿using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector2Int gridPosition);
    void UpdateState(Vector2Int gridPosition);
}