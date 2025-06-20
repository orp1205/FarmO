using System;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    //[SerializeField]
    //private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;
    private Material cellIndicatorMaterial;

    private SpriteRenderer cellIndicatorRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<SpriteRenderer>();
        cellIndicatorMaterial = cellIndicatorRenderer.material;
    }
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }
    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
    private void PreparePreview(GameObject previewObject)
    {
        SpriteRenderer render = previewObject.GetComponentInChildren<SpriteRenderer>();
        render.material = previewMaterialInstance;
    }

    private void PrepareCursor(Vector2Int size)
    {
        if(size.x>0|| size.y>0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, size.y, 1);
            Vector2 tilling = new Vector2(size.x, size.y);
            cellIndicatorRenderer.material.SetVector("_Tilling", tilling);
        }
    }
    public void StopShowingPlacementPreview()
    {
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }
    public void UpdatePositionPlacementPreview(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);

        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y, position.z);
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }
    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
