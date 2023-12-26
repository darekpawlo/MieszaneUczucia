using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;


public class ManagerMenu : MonoBehaviour
{
    [Header("Adding menu item")]
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField priceInput;
    [SerializeField] TMP_Dropdown blockedDropdown;
    [SerializeField] TMP_InputField descriptionInput;
    [SerializeField] Transform addItemPanel;
    Texture2D iconTexture;

    [Header("Modifying menu item")]
    [SerializeField] TMP_InputField itemNameInput;
    [SerializeField] TMP_InputField itemPriceInput;
    [SerializeField] TMP_InputField itemDescriptionInput;
    [SerializeField] TMP_Dropdown itemBlockedDropdown;
    [SerializeField] Image itemIcon;

    [SerializeField] MenuItem menuItemPrefab;
    [SerializeField] Transform itemPanel;
    [SerializeField] Transform configureMenuPanel;
    [SerializeField] Transform menuItemHolder; 
    List<MenuItem> activeMenuItems = new List<MenuItem>();
    MenuItemData activeItemData;

    public void SelectIcon(Image iconPreview)
    {
        int maxSize = 256;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                iconPreview.sprite = ImageManager.TextureToSprite(texture);
                iconTexture = ImageManager.CreateReadableTexture(texture);
            }
        });

        Debug.Log("Permission result: " + permission);
    }
    public void CreateMenuItem()
    {
        StartCoroutine(WebActions.InsertMenuOracle(nameInput.text, priceInput.text, blockedDropdown.value.ToString(), descriptionInput.text, ImageManager.TextureToString(iconTexture), (text) =>
        {
            ShowMenu();
            configureMenuPanel.gameObject.SetActive(true);
            addItemPanel.gameObject.SetActive(false);
        }));
    }

    public void ShowMenu()
    {
        foreach (MenuItem item in activeMenuItems)
        {
            Destroy(item.gameObject);
        }
        activeMenuItems.Clear();

        StartCoroutine(WebActions.GetMenuOracle((menuItemJson) =>
        {
            var menuItems = JsonConvert.DeserializeObject<List<MenuItemJsonData>>(menuItemJson);
            for (int i = 0; i < menuItems.Count; i++)
            {
                var item = menuItems[i];
                var data = new MenuItemData(item.Id, item.Name, item.Description, item.Blocked, item.Price, ImageManager.BytesToSprite(Convert.FromBase64String(item.Icon)));
                var menuItem = Instantiate(menuItemPrefab, transform.position, Quaternion.identity, menuItemHolder);
                activeMenuItems.Add(menuItem);

                menuItem.Init(data, itemPanel, (itemData) =>
                {
                    itemNameInput.text = item.Name;
                    itemPriceInput.text = item.Price;
                    itemDescriptionInput.text = item.Description;
                    itemBlockedDropdown.value = item.Blocked == "0" ? 0 : 1;
                    itemIcon.sprite = itemData.Icon;

                    activeItemData = itemData;
                });
            }
        }));
    }

    public void DeleteItem()
    {
        StartCoroutine(WebActions.DeleteMenuOracle(activeItemData.Id, (text) =>
        {
            itemPanel.gameObject.SetActive(false);
            ShowMenu();
        }));
    }

    public void UpdateItem()
    {
        StartCoroutine(WebActions.UpdateMenuOracle(activeItemData.Id, itemNameInput.text, itemPriceInput.text, itemBlockedDropdown.value.ToString(), itemDescriptionInput.text, ImageManager.TextureToString(iconTexture), (text) =>
        {
            itemPanel.gameObject.SetActive(false);
            ShowMenu();                       
        }));
    }
    public void MenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
