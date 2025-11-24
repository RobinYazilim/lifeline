using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager inst;
    public bool shopVisible = false;
    private Vector2 endGoal = new Vector2(273.2102f, 56.37762f);
    private RectTransform rectTransform;
    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        rectTransform = transform.Find("Frame").GetComponent<RectTransform>();
    }

    public void showShop()
    {
        shopVisible = true;
        endGoal = new Vector2(-273.2102f, 56.37762f);
    }

    public void hideShop()
    {
        shopVisible = false;
        endGoal = new Vector2(273.2102f, 56.37762f);
    }

    public void Update()
    {
        // asiri performans sikcek ama olsun
        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, endGoal, Time.deltaTime * 2f);
    }
}
