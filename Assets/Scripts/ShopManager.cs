using System.Data.SqlTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager inst;
    private Vector2 endGoal = new Vector2(273.2102f, 56.37762f);
    private Vector2 shopBtnEndGoal = new Vector2(-113.9971f, -66.99463f);
    private Vector2 moneyEndGoal = new Vector2(-322f, -66.99463f);
    private RectTransform rectTransform;
    private RectTransform shopBtnRectTransform;
    private RectTransform moneyRectTransform;
    private Button shopOpenButton;
    private float speed = 5f;
    public int money = 10;
    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    (string name, TurretType type, int price)[] turrets = new (string, TurretType, int)[]
    {
        ("Basic", TurretType.Basic, 10),
        ("AOE", TurretType.AOE, 20),
        ("Stun", TurretType.Stun, 15),
        ("AllInOne", TurretType.AllInOne, 30),
        ("Buff", TurretType.Buff, 1),
        ("Debuff", TurretType.Debuff, 10)
    };

    private void Start()
    {
        rectTransform = transform.Find("Frame").GetComponent<RectTransform>();
        shopBtnRectTransform = transform.Find("Open").GetComponent<RectTransform>();
        moneyRectTransform = transform.Find("Money").GetComponent<RectTransform>();
        shopOpenButton = transform.Find("Open").GetComponent<Button>();
        shopOpenButton.onClick.AddListener(() =>
        {
            showShop();
        });

        

        foreach (var (name, type, price) in turrets)
        {
            Button btn = transform.Find($"Frame/{name}").GetComponent<Button>();
            TextMeshProUGUI textComp = btn.transform.Find("InnerText").GetComponent<TextMeshProUGUI>();
            textComp.text = $"{name}\n${price}";
            btn.onClick.AddListener(() =>
            {
                if (money < price) return;
                money -= price;
                MouseHandler.inst.BuyTurret(type);
                hideShop();
            });
        }
    }

    public void showShop()
    {
        endGoal = new Vector2(-273.2102f, 56.37762f);
        shopBtnEndGoal = new Vector2(-113.9971f, 66.99463f);
        moneyEndGoal = new Vector2(-642.41715f, -66.99463f);
    }

    public void hideShop()
    {
        endGoal = new Vector2(273.2102f, 56.37762f);
        shopBtnEndGoal = new Vector2(-113.9971f, -66.99463f);
        moneyEndGoal = new Vector2(-322f, -66.99463f);
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
