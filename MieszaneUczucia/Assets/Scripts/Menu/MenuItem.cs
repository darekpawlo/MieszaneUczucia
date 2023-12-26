using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{   
    public MenuItemData Data;

    public void Init(MenuItemData data, Transform itemPanel, Action<MenuItemData> callBack = null)
    {
        Data = data;

        transform.Find("Name").GetComponent<TMP_Text>().text = data.Name;
        transform.Find("Price").GetComponent<TMP_Text>().text = data.Price.ToString("F");
        transform.Find("Description").GetComponent<TMP_Text>().text = data.Description;
        transform.Find("Icon").GetComponent<Image>().sprite = data.Icon;
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            itemPanel.gameObject.SetActive(true);

            callBack?.Invoke(data);
        });
    }
}

[Serializable]
public class MenuItemData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Blocked { get; private set; }    
    public float Price
    {
        get
        {
            return _price / 100;
        }

        private set
        {
            _price = value;
        }
    }
    public Sprite Icon { get; private set; }

    private float _price;

    public MenuItemData(string id, string name, string description, string blocked, string price, Sprite icon)
    {
        Id = id;
        Name = name;
        Description = description;
        Blocked = blocked;
        Price = int.Parse(price);
        Icon = icon;
    }
}
