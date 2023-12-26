using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField] MenuItem menuItemPrefab;
    [SerializeField] Transform menuItemHolder;
    [SerializeField] Transform menuPanel;

    [Header("Scripts")]
    [SerializeField] OrderManager orderManager;

    public static List<MenuItemData> MenuItemsData { get; private set; } = new List<MenuItemData>();

    private void Awake()
    {
        Instance = this;

        CreateItemRoutine();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        menuPanel.gameObject.SetActive(true);
    }

    void CreateItemRoutine()
    {
        StartCoroutine(WebActions.GetMenuOracle((menuItemJson) =>
        {
            var menuItems = JsonConvert.DeserializeObject<List<MenuItemJsonData>>(menuItemJson);
            MenuItemsData.Clear();
            foreach (var item in menuItems)
            {
                var data = new MenuItemData(item.Id, item.Name, item.Description, item.Blocked, item.Price, ImageManager.BytesToSprite(Convert.FromBase64String(item.Icon)));
                MenuItemsData.Add(data);
            }

            for (int i = 0; i < MenuItemsData.Count; i++)
            {
                var menuItem = Instantiate(menuItemPrefab, transform.position, Quaternion.identity, menuItemHolder);
                menuItem.Init(MenuItemsData[i], orderManager.GetOrderPanel, (itemData) =>
                {
                    orderManager.SetOrderValues(itemData);
                    orderManager.SetBottomBar();
                });                
            }
        }));
    }

    public void GoToLogin() => SceneManager.LoadScene("LoginMenu");
    public void MenuPanelState(bool state) => menuPanel.gameObject.SetActive(state);
}

[Serializable]
public struct MenuItemJsonData
{
    [JsonProperty("ID_pozycji")]
    public string Id;

    [JsonProperty("Nazwa produktu")]
    public string Name;

    [JsonProperty("Cena")]
    public string Price;

    [JsonProperty("Zablokowane")]
    public string Blocked;

    [JsonProperty("Skladniki")]
    public string Description;

    [JsonProperty("Zdjecia")]
    public string Icon;
}

