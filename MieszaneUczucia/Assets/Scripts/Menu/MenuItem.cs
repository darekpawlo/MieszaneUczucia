using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour
{   
    public MenuItemData Data;  
}

[Serializable]
public class MenuItemData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Price { get; private set; }
    public Sprite Icon { get; private set; }

    public MenuItemData(string id, string name, string description, string price)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
    }

    public void SetIcon(Sprite icon) => Icon = icon;
}
