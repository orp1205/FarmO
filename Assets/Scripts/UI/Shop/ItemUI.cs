using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, ShopItem
{
    public ItemUIVisual ItemUIVisual;
    private FarmingObject Item;
    private WorkerData Worker;
    public FarmingObject item
    {
        get { return Item; }
        set { Item = value; }
    }
    public WorkerData worker
    {
        get { return Worker; }
        set { Worker = value; }
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
        ItemUIVisual.transform.DOMove(worldTarget, 0.5f).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {

            });
    }
    public void SetUIShopItem(int TypeOfShop)
    {
        if (TypeOfShop < 2)
        {
            ItemUIVisual.SetUIShopItem(Item, TypeOfShop);
        }
        else
        {
            ItemUIVisual.SetUIShopWorker(Worker, TypeOfShop);
        }
        StartCoroutine(AnimateToCorrectPosition());
    }

    public void UpdateItemStateBuy(bool canBuy)
    {
        ItemUIVisual.UpdateItemState(canBuy);
    }
    public void UpdateItemStateSell(bool canSell)
    {
        ItemUIVisual.gameObject.SetActive(canSell); // Ensure the item UI is active when updating state
        ItemUIVisual.UpdateItemState(canSell);
    }
    public void UpdateItemStateBuyWorker(bool canBuy)
    {
        ItemUIVisual.gameObject.SetActive(canBuy); // Ensure the item UI is active when updating state
        ItemUIVisual.UpdateItemState(canBuy);
    }
    public void OnClickItem(Action OnclickAction)
    {
        ItemUIVisual.OnClickItem(OnclickAction);
    }
    private void OnDestroy()
    {
        Destroy(ItemUIVisual.gameObject); // Ensure the visual is destroyed when the item UI is destroyed   
    }

}
