using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ConfigurationItem : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Price;
    public TMP_Text Amount;
    public Button AddButton;
    public Button RemoveButton;
    public Transform Blocked;

    public ConfigurationItemData data;
    public int amount = 0;

    public void Init(ConfigurationItemData data, Action callBack)
    {
        this.data = data;
        Amount.text = 0.ToString();

        Name.text = data.Produkt;
        Price.text = data.Cena.ToString("F");

        Blocked.gameObject.SetActive(data.Zablokowane == 0 ? false : true);

        AddButton.onClick.AddListener(() =>
        {
            amount += 1;
            OrderManager.orderPrice += data.Cena;

            UpdateAmountText();
            callBack();
        });

        RemoveButton.onClick.AddListener(() =>
        {
            if (amount <= 0) return;
            amount -= 1;
            OrderManager.orderPrice -= data.Cena;

            UpdateAmountText();
            callBack();
        });
    }

    void UpdateAmountText() => Amount.text = amount.ToString();
}
