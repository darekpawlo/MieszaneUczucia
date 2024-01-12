using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order
{
    public float OrderPrice;
    public MenuItemOrder MenuItem;
    public List<ConfigurationItemOrder> ConfigurationItems = new List<ConfigurationItemOrder>();

    public Order(float orderPrice, MenuItemOrder menuItem, List<ConfigurationItemOrder> configurationItems)
    {
        OrderPrice = orderPrice;
        MenuItem = menuItem;
        ConfigurationItems = configurationItems;
    }

    public float CalculatePrice()
    {
        float price = MenuItem.Amount * MenuItem.Price;

        foreach (ConfigurationItemOrder conf in ConfigurationItems)
        {
            price += conf.Price * conf.Amount;
        }

        return price;
    }
}

[System.Serializable]
public class MenuItemOrder
{
    public string Name;
    public float Price;
    public int Amount;

    public MenuItemOrder(string name, int amount, float price)
    {
        Amount = amount;
        Name = name;
        Price = price;
    }
}

[System.Serializable]
public class ConfigurationItemOrder
{
    public string Name;
    public int Amount;
    public float Price;

    public ConfigurationItemOrder(string name, int amount, float price)
    {
        Name = name;
        Amount = amount;
        Price = price;
    }
}
