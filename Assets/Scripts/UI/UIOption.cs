using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : MonoBehaviour
{
    public UIOptionVisual visual;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SetDataBuilding(ObjectData data)
    {
        visual.SetDataBuilding(data); // Assuming UIOptionVisual has a SetData method that takes ObjectData

        StartCoroutine(AnimateToCorrectPosition()); // Start the animation coroutine to position the visual correctly
    }
    public void SetDataFarm(FarmingObject data, FarmSlot slot)
    {
        visual.SetDataFarm(data, slot);
        StartCoroutine(AnimateToCorrectPosition()); // Start the animation coroutine to position the visual correctly
    }

    public void SetInfoName(string name)
    {
        visual.SetUIFarmSlotName(name); // Assuming UIOptionVisual has a method to set the level of the farming slot
        StartCoroutine(AnimateToCorrectPosition()); // Start the animation coroutine to position the visual correctly
    }
    public void SetUIFarmSlotProduct(FarmSlot slot)
    {
        visual.SetUIFarmSlotProduct(slot); // Assuming UIOptionVisual has a method to set the product info
        StartCoroutine(AnimateToCorrectPosition()); // Start the animation coroutine to position the visual correctly
    }
    public void SetUIDeleteFarmObject(FarmSlot slot)
    {
        visual.SetUIDeleteFarmObject(slot); // Assuming UIOptionVisual has a method to set the status of the farming slot
        StartCoroutine(AnimateToCorrectPosition()); // Start the animation coroutine to position the visual correctly
    }
    public void SetCloseFarmSlotStatus()
    {
        visual.SetCloseFarmSlotStatus(); // Assuming UIOptionVisual has a method to set the status of the farming slot
        StartCoroutine(AnimateToCorrectPosition()); // Start the animation coroutine to position the visual correctly
    }
    public Vector3 GetWorldCenter()
    {
        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this.gameObject.GetComponent<RectTransform>());
        return this.transform.TransformPoint(bounds.center);
    }
    IEnumerator AnimateToCorrectPosition()
    {
        yield return null;

        Vector3 worldTarget = GetWorldCenter();
        visual.transform.DOMove(worldTarget, 0.5f).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                
            });
    }

    public void SetDestructionButton()
    {
        visual.SetDestructionButton();
        StartCoroutine(AnimateToCorrectPosition()); // Start the animation coroutine to position the visual correctly
    }
}
