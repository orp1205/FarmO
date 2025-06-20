using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastMousePosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action Onclicked, OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Onclicked?.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnExit?.Invoke();
        }
    }
    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();
    public Vector2 GetSelectedMapPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -sceneCamera.transform.position.z; // For orthographic camera, set z so world point is on camera's plane
        Vector3 worldPosition = sceneCamera.ScreenToWorldPoint(mouseScreenPosition);
        return new Vector2(worldPosition.x, worldPosition.y);
    }
}
