using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [SerializeField] Transform orderPanel;
    [SerializeField] Transform itemPanel;
    [SerializeField] Transform[] extraItemPanel;

    private void Start()
    {
        OrderPanelState(false);
    }

    public void SetOrderValues(MenuItemData data)
    {
        itemPanel.Find("Icon").GetComponent<Image>().sprite = data.Icon;
        itemPanel.Find("Name").GetComponent<TMP_Text>().SetText(data.Name);
        itemPanel.Find("Price").GetComponent<TMP_Text>().SetText(data.Price);
        itemPanel.Find("Description").GetComponent<TMP_Text>().SetText(data.Description);
    }

    public void SetExtrasValues(MenuItemData data, int extraId)
    {
        extraItemPanel[extraId].Find("Icon").GetComponent<Image>().sprite = data.Icon;
        extraItemPanel[extraId].Find("Name").GetComponent<TMP_Text>().SetText(data.Name);
        extraItemPanel[extraId].Find("Price").GetComponent<TMP_Text>().SetText(data.Price);
    }

    public void OrderPanelState(bool state) => orderPanel.gameObject.SetActive(state);
}
