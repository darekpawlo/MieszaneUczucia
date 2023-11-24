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
    [SerializeField] MenuItem menuItemPrefab;
    [SerializeField] Transform menuItemHolder;
    [SerializeField] Transform menuPanel;

    [Header("Scripts")]
    [SerializeField] OrderManager orderManager;

    List<MenuItemData> menuItemsData = new List<MenuItemData>();

    private void Start()
    {
        Application.targetFrameRate = 60;
        menuPanel.gameObject.SetActive(true);

        CreateItemRoutine();
    }

    void CreateItemRoutine()
    {
        void SetImage(MenuItem menuItem, string itemId)
        {
            byte[] bytes = ImageManager.LoadImage(itemId);
            //Download from web
            if (bytes.Length == 0)
            {
                StartCoroutine(WebActions.GetItemIcon(itemId, (downloadBytes) =>
                {
                    Sprite icon = ImageManager.BytesToSprite(downloadBytes);                    
                    ImageManager.SaveImage(itemId, downloadBytes);
                    menuItem.transform.Find("Icon").GetComponent<Image>().sprite = icon;
                    menuItem.Data.SetIcon(icon);
                }));                
            }
            //Load from device
            else
            {
                Sprite icon = ImageManager.BytesToSprite(bytes);
                menuItem.transform.Find("Icon").GetComponent<Image>().sprite = icon;
                menuItem.Data.SetIcon(icon);
            }
        }

        void SetImageData(MenuItemData data)
        {
            byte[] bytes = ImageManager.LoadImage(data.Id);
            //Download from web
            if (bytes.Length == 0)
            {
                StartCoroutine(WebActions.GetItemIcon(data.Id, (downloadBytes) =>
                {
                    Sprite icon = ImageManager.BytesToSprite(downloadBytes);
                    ImageManager.SaveImage(data.Id, downloadBytes);
                    data.SetIcon(icon);
                }));
            }
            //Load from device
            else
            {
                Sprite icon = ImageManager.BytesToSprite(bytes);
                data.SetIcon(icon);
            }
        }

        StartCoroutine(WebActions.GetMenu((menuItemJson) =>
        {
            var menuItems = JsonConvert.DeserializeObject<List<MenuItemJsonData>>(menuItemJson);
            foreach (var item in menuItems)
            {
                var data = new MenuItemData(item.Id, item.Name, item.Description, item.Price);
                SetImageData(data);
                menuItemsData.Add(data);
            }

            for (int i = 0; i < 10; i++)
            {
                var menuItem = Instantiate(menuItemPrefab, transform.position, Quaternion.identity, menuItemHolder);
                var itemData = menuItemsData[i];

                menuItem.Data = itemData;
                menuItem.transform.Find("Name").GetComponent<TMP_Text>().text = itemData.Name;
                menuItem.transform.Find("Price").GetComponent<TMP_Text>().text = itemData.Price;
                menuItem.transform.Find("Description").GetComponent<TMP_Text>().text = itemData.Description;
                SetImage(menuItem, itemData.Id);
                //menuItem.transform.Find("Icon").GetComponent<Image>().sprite = itemData.Icon;
                menuItem.transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    MenuPanelState(false);
                    orderManager.OrderPanelState(true);

                    orderManager.SetOrderValues(itemData);                    
                    orderManager.SetExtrasValues(menuItemsData[10], 0);
                    orderManager.SetExtrasValues(menuItemsData[11], 1);
                    orderManager.SetExtrasValues(menuItemsData[12], 2);
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
    public string Id;
    public string Name;
    public string Description;
    public string Price;
}

