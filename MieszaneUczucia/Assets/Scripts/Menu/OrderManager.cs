using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [SerializeField] Transform orderPanel;
    [SerializeField] Transform itemPanel;
    [SerializeField] Transform[] extraItemPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OrderPanelState(false);
    }

    public void SetOrderValues(MenuItemData data)
    {
        itemPanel.Find("Icon").GetComponent<Image>().sprite = data.Icon;
        itemPanel.Find("Name").GetComponent<TMP_Text>().SetText(data.Name);
        itemPanel.Find("Price").GetComponent<TMP_Text>().SetText(data.Price.ToString("F"));
        itemPanel.Find("Description").GetComponent<TMP_Text>().SetText(data.Description);
    }

    public void SetExtrasValues()
    {
        for (int i = 0; i < extraItemPanel.Length; i++)
        {
            var data = MenuManager.MenuItemsData[Random.Range(0, MenuManager.MenuItemsData.Count)];

            extraItemPanel[i].Find("Icon").GetComponent<Image>().sprite = data.Icon;
            extraItemPanel[i].Find("Name").GetComponent<TMP_Text>().SetText(data.Name);
            extraItemPanel[i].Find("Price").GetComponent<TMP_Text>().SetText(data.Price.ToString("F"));
        }
    }

    public void OrderPanelState(bool state) => orderPanel.gameObject.SetActive(state);
}
