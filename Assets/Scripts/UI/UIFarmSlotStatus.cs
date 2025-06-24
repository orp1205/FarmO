using UnityEngine;
using UnityEngine.UI;

public class UIFarmSlotStatus : MonoBehaviour
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

    public void SetFill(float fillAmount, bool fullProduction)
    {
        Fill.fillAmount = fillAmount; // Set the fill amount of the UI element
        if (fullProduction)
        {
            Fill.color = Color.red; // Change color to green if full production
        }
        else
        {
            Fill.color = Color.green; // Change color to yellow if not full production
        }
    }

    public void SetIcon(Sprite icon)
    {
        Icon.sprite = icon; // Set the icon sprite for the UI element
    }
}
