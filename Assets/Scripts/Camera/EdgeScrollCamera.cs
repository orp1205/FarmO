using UnityEngine;

public class EdgeScrollSmoothCamera : MonoBehaviour
{
    public float scrollSpeed = 10f;         
    public int edgeSize = 20;
    public Vector2 minBounds;   // Minimum X/Y position
    public Vector2 maxBounds;   // Maximum X/Y position
    public float smoothTime = 0.15f;

    [SerializeField]
    private CameraFollow cameraFollow;

    private Vector3 cameraFollowPosition;
    private void Start()
    {
        cameraFollow.Setup(() => cameraFollowPosition, () => 80f, true, false);
    }

    private void Update()
    {
        bool wasMoving = IsMouseOnEdge();

        MoveByXSide();
        MoveByYSide();

        // If not moving, force camera to snap to position
        if (!wasMoving)
        {
            cameraFollow.Setup(() => cameraFollowPosition, () => 0f, false, false); // teleportToFollowPosition = true
        }
    }
    private bool IsMouseOnEdge()
    {
        return Input.mousePosition.x < edgeSize ||
               Input.mousePosition.x > Screen.width - edgeSize ||
               Input.mousePosition.y < edgeSize ||
               Input.mousePosition.y > Screen.height - edgeSize;
    }
    private void MoveByXSide()
    {
        float newX = cameraFollowPosition.x;
        bool moved = false;

        if (Input.mousePosition.x < edgeSize)
        {
            newX -= scrollSpeed * Time.deltaTime;
            moved = true;
        }
        else if (Input.mousePosition.x > Screen.width - edgeSize)
        {
            newX += scrollSpeed * Time.deltaTime;
            moved = true;
        }

        if (moved)
        {
            cameraFollowPosition.x = Mathf.Clamp(newX, minBounds.x, maxBounds.x);
        }
    }
    private void MoveByYSide()
    {
        float newY = cameraFollowPosition.y;
        bool moved = false;

        if (Input.mousePosition.y < edgeSize)
        {
            newY -= scrollSpeed * Time.deltaTime;
            moved = true;
        }
        else if (Input.mousePosition.y > Screen.height - edgeSize)
        {
            newY += scrollSpeed * Time.deltaTime;
            moved = true;
        }

        if (moved)
        {
            cameraFollowPosition.y = Mathf.Clamp(newY, minBounds.y, maxBounds.y);
        }
    }
}
