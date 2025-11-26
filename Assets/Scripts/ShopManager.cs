using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager inst;
    public bool shopVisible = false;
    private Vector2 endGoal = new Vector2(273.2102f, 56.37762f);
    private Vector2 shopBtnEndGoal = new Vector2(-113.9971f, -66.99463f);
    private Vector2 moneyEndGoal = new Vector2(-322f, -66.99463f);
    private RectTransform rectTransform;
    private RectTransform shopBtnRectTransform;
    private RectTransform moneyRectTransform;
    private float speed = 5f;
    private bool skip = false;
    private bool canSkip = false;
    public int money = 10;
    private int enemyCount = 0;
    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    (string name, TurretType type, int price)[] turrets = new (string, TurretType, int)[]
    {
        ("Basic", TurretType.Basic, 10),
        ("AOE", TurretType.AOE, 30),
        ("Stun", TurretType.Stun, 45),
        ("AllInOne", TurretType.AllInOne, 100),
        ("Buff", TurretType.Buff, 5),
        ("Debuff", TurretType.Debuff, 20)
    };

    public void removeEnemyCount()
    {
        enemyCount--;
        transform.Find("Counter/InnerText").GetComponent<TextMeshProUGUI>().text = $"Enemies left: {enemyCount}";
    }
    public void setEnemyCount(int num)
    {
        enemyCount = num;
        transform.Find("Counter/InnerText").GetComponent<TextMeshProUGUI>().text = $"Enemies left: {enemyCount}";
    }

    public IEnumerator timerCoroutine(float duration)
    {
        float timeLeft = duration;
        TextMeshProUGUI text = transform.Find("Counter/InnerText").GetComponent<TextMeshProUGUI>();
        canSkip = true;
        while (timeLeft > 0)
        {
            if (skip)
            {
                skip = false;
                break;
            }
            text.text = $"Next wave in: {timeLeft:F1}s";
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        canSkip = false;

        text.text = $"Next wave starting...";
    }

    private void Start()
    {
        rectTransform = transform.Find("Frame").GetComponent<RectTransform>();
        shopBtnRectTransform = transform.Find("Open").GetComponent<RectTransform>();
        moneyRectTransform = transform.Find("Money").GetComponent<RectTransform>();
        transform.Find("Open").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (MouseHandler.inst.turretbought)
                return;
            showShop();
        });

        transform.Find("Frame/Close").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (MouseHandler.inst.turretbought)
                return;
            hideShop();
        });
        transform.Find("Frame/Skip").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (MouseHandler.inst.turretbought || !canSkip)
                return;
            skip = true;
            hideShop();
        });
        

        foreach (var (name, type, price) in turrets)
        {
            Button btn = transform.Find($"Frame/{name}").GetComponent<Button>();
            TextMeshProUGUI textComp = btn.transform.Find("InnerText").GetComponent<TextMeshProUGUI>();
            textComp.text = $"{name}\n${price}";
            int ogPrice = price;
            int editablePrice = price;
            int boughtCount = 0;
            btn.onClick.AddListener(() =>
            {
                if (MouseHandler.inst.turretbought)
                    return;
                if (money < editablePrice) return;
                money -= editablePrice;
                boughtCount++;
                editablePrice = ogPrice + ogPrice/2 * boughtCount;
                MouseHandler.inst.BuyTurret(type);
                hideShopPurchaseMode();
                textComp.text = $"{name}\n${editablePrice}";
            });
        }
    }

    public void showShop()
    {
        shopVisible = true;
        endGoal = new Vector2(-273.2102f, 56.37762f);
        shopBtnEndGoal = new Vector2(-113.9971f, 66.99463f);
        moneyEndGoal = new Vector2(-642.41715f, -66.99463f);
    }

    public void hideShop()
    {
        shopVisible = false;
        endGoal = new Vector2(273.2102f, 56.37762f);
        shopBtnEndGoal = new Vector2(-113.9971f, -66.99463f);
        moneyEndGoal = new Vector2(-322f, -66.99463f);
    }
    
    public void hideShopPurchaseMode()
    {
        endGoal = new Vector2(273.2102f, 56.37762f);
        shopBtnEndGoal = new Vector2(-113.9971f, 66.99463f);
        moneyEndGoal = new Vector2(-113.9971f, -66.99463f);
    }

    public void Update()
    {
        // asiri performans sikcek ama olsun
        moneyRectTransform.Find("InnerText").GetComponent<TextMeshProUGUI>().text = $"${money}";

        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, endGoal, Time.deltaTime * speed);
        shopBtnRectTransform.anchoredPosition = Vector2.Lerp(shopBtnRectTransform.anchoredPosition, shopBtnEndGoal, Time.deltaTime * speed);
        moneyRectTransform.anchoredPosition = Vector2.Lerp(moneyRectTransform.anchoredPosition, moneyEndGoal, Time.deltaTime * speed);
    }
}
