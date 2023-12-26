using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    public static float TotalAmount 
    { 
        get => totalAmount; 

        set
        {
            totalAmount = value;

            if (totalAmount < 0) totalAmount = 0;
        } 
    }
    private static float totalAmount;

    [SerializeField] Transform orderPanel;
    [SerializeField] Transform item;

    [Header("BottomBar")]
    [SerializeField] TMP_Text priceText;
    [SerializeField] TMP_Text amountText;
    int itemAmount = 1;
    MenuItemData activeItem;

    private void Start()
    {
        OrderPanelState(false);
    }

    public void Init()
    {

    }

    public void SetOrderValues(MenuItemData data)
    {
        activeItem = data;

        item.Find("Icon").GetComponent<Image>().sprite = data.Icon;
        item.Find("Name").GetComponent<TMP_Text>().SetText(data.Name);
        item.Find("Price").GetComponent<TMP_Text>().SetText(data.Price.ToString("F"));
        item.Find("Description").GetComponent<TMP_Text>().SetText(data.Description);
    }

    public void SetBottomBar()
    {
        TotalAmount = activeItem.Price * itemAmount;
        priceText.text = TotalAmount.ToString("F");
        amountText.text = $"{itemAmount}";
    }

    public void IncreaseAmount(int amount)
    {
        if (itemAmount <= 1 && amount <= -1) return;
        itemAmount += amount;
        SetBottomBar();
    }

    public void OrderPanelState(bool state) => orderPanel.gameObject.SetActive(state);

    public Transform GetOrderPanel => orderPanel;

    
}


[Serializable]
public struct ConfigurationJson
{
    public string Produkt;
    public int? Cena; 

    public ConfigurationJson(string produkt, int? cena)
    {
        Produkt = produkt;
        Cena = cena;
    }
}