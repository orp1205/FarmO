using UnityEngine;
using UnityEngine.UI;

public class UIWorkerStatus : MonoBehaviour
{
    [SerializeField]
    private Image Fill;
    [SerializeField]
    private Image Icon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFill(float fillAmount)
    {
        Fill.fillAmount = fillAmount; // Set the fill amount of the UI element
    }

    public void SetIcon(Sprite icon)
    {
        Icon.sprite = icon; // Set the icon sprite for the UI element
    }
}
