using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuItem menuItemPrefab;

    private void Start()
    {
        CreateItemRoutine();
    }

    void CreateItemRoutine()
    {
        void SetImage(GameObject menuItem, string itemId)
        {
            byte[] bytes = ImageManager.LoadImage(itemId);
            //Download from web
            if (bytes.Length == 0)
            {
                StartCoroutine(WebActions.GetItemIcon(itemId, (downloadBytes) =>
                {
                    menuItem.transform.Find("Icon").GetComponent<Image>().sprite = ImageManager.BytesToSprite(downloadBytes);
                    ImageManager.SaveImage(itemId, downloadBytes);
                }));
            }
            //Load from device
            else
            {
                menuItem.transform.Find("Icon").GetComponent<Image>().sprite = ImageManager.BytesToSprite(bytes);
            }
        }

        StartCoroutine(WebActions.GetMenu((menuItemJson) =>
        {
            var menuItems = JsonConvert.DeserializeObject<List<MenuItemData>>(menuItemJson);
            foreach (var menuItemData in menuItems)
            {
                var menuItem = Instantiate(menuItemPrefab, transform.position, Quaternion.identity, transform);
                menuItem.Init(menuItemData.id, menuItemData.name, menuItemData.description, menuItemData.price);

                menuItem.transform.Find("Name").GetComponent<TMP_Text>().text = menuItem.Name;
                menuItem.transform.Find("Price").GetComponent<TMP_Text>().text = menuItem.Price;
                menuItem.transform.Find("Description").GetComponent<TMP_Text>().text = menuItem.Description;
                SetImage(menuItem.gameObject, menuItem.Id);
            }          
        }));
    }
}

struct MenuItemData
{
    public string id;
    public string name;
    public string description;
    public string price;
}
