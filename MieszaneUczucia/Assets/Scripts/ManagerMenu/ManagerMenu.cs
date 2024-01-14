using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using static UnityEditor.Progress;


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

    [Header("Configuration item")]
    [SerializeField] Transform configurationItemPanel;
    [SerializeField] TMP_InputField configurationNameInput;
    [SerializeField] TMP_InputField configurationPriceInput;
    [SerializeField] GameObject configurationLayoutGroup;
    [SerializeField] GameObject itemToConfigure;
    List<ItemToConfigure> spawnedItemToConfigure = new List<ItemToConfigure>();
    int amountOfDoneOperations = 0;
    int amountOfOperations = 0;

    [Header("ConfigureConfiguration")]
    [SerializeField] Transform confConfigurationLayoutGroup;
    [SerializeField] GameObject configurationItem;
    [SerializeField] Transform confConfigurationPanel;
    List<GameObject> spawnedConfigurationItems = new List<GameObject>();
    [SerializeField] Transform confSpecificConfigurationPanel;
    [SerializeField] Transform specificConfLayoutGroup;
    [SerializeField] TMP_InputField specificConfNameInput;
    [SerializeField] TMP_InputField specificConfPriceInput;
    List<GameObject> spawnedSpecificConfItem = new List<GameObject>();
    ConfigurationItemOnlyJson activeConfiguration;

    List<MenuItem> activeMenuItems = new List<MenuItem>();
    MenuItemData activeItemData;

    TaskManager taskManager = new TaskManager();

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

    public void ConfigurationInit()
    {
        configurationItemPanel.gameObject.SetActive(true);

        if (spawnedItemToConfigure.Count > 0) spawnedItemToConfigure.ForEach(item => { Destroy(item.gameObject); });
        spawnedItemToConfigure.Clear();

        Prompt.Instance.ShowLoadingBar();
        CallWebActionGet();
    }

    public void CreateConfiguration()
    {
        if (taskManager.IsTaskRunning) return;
        Prompt.Instance.ShowLoadingBar();

        amountOfDoneOperations = 0;
        amountOfOperations = 0;
        foreach (var item in spawnedItemToConfigure)
        {
            if (item.ActiveCheckmark) amountOfOperations++;
        }
        CallWebActionInsert();
    }

    async void CallWebActionInsert()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.InsertKonfiguracjeOracleAsync(configurationNameInput.text, configurationPriceInput.text, cancellationToken, (text) =>
                {
                    tcs.SetResult(text);
                });

                string responseText;
                try
                {
                    responseText = await tcs.Task;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Operation was cancelled.");
                    return; // Early exit if the operation is cancelled
                }

                if (responseText.Contains("Error"))
                {
                    Prompt.Instance.ShowTooltip(responseText);
                }
                else
                {
                    configurationNameInput.text = string.Empty;
                    configurationPriceInput.text = string.Empty;

                    foreach (var item in spawnedItemToConfigure)
                    {
                        if (!item.ActiveCheckmark) continue;
                        item.TurnCheckmarksOff();

                        StartCoroutine(WebActions.InsertOptionsConfigurationOracle(item.Data.ID_pozycji, responseText, (text) =>
                        {
                            if (text.Contains("Error"))
                            {
                                Prompt.Instance.ShowTooltip(text);
                                Debug.Log(text);
                            }
                            else
                            {
                                Debug.Log(text);
                                amountOfDoneOperations++;
                                if (amountOfDoneOperations >= amountOfOperations) Prompt.Instance.ShowTooltip("Dodano konfiguracje!");
                            }
                        }));
                    }
                }
            }
        });
    }


    async void CallWebActionGet()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetMenuOracleAsync_Id_Name(cancellationToken, (text) =>
                {
                    tcs.SetResult(text);
                });

                string responseText;
                try
                {
                    responseText = await tcs.Task;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Operation was cancelled.");
                    return; // Early exit if the operation is cancelled
                }

                List<MenuItemIdName> list = JsonConvert.DeserializeObject<List<MenuItemIdName>>(responseText);

                foreach (var item in list)
                {
                    var spawnedItem = Instantiate(itemToConfigure, configurationLayoutGroup.transform.position, Quaternion.identity, configurationLayoutGroup.transform);
                    spawnedItem.gameObject.SetActive(true);

                    var text = spawnedItem.transform.Find("Text").GetComponent<TMP_Text>();
                    var button = spawnedItem.GetComponent<Button>();
                    var checkmark = spawnedItem.transform.Find("CheckmarkBackground").GetChild(0);

                    var createdItem = spawnedItem.AddComponent<ItemToConfigure>();
                    createdItem.Init(item, text, checkmark, button);

                    spawnedItemToConfigure.Add(createdItem);
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }

    async void CallWebActionGetConfiguration()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetOnlyConfigurationOracle(cancellationToken, (text) =>
                {
                    tcs.SetResult(text);
                });

                string responseText;
                try
                {
                    responseText = await tcs.Task;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Operation was cancelled.");
                    return; // Early exit if the operation is cancelled
                }

                if (responseText.Contains("error"))
                {
                    Prompt.Instance.ShowTooltip(responseText);
                    return;
                }

                List<ConfigurationItemOnlyJson> list = JsonConvert.DeserializeObject<List<ConfigurationItemOnlyJson>>(responseText);

                foreach (var item in list)
                {
                    var confItem = Instantiate(configurationItem, confConfigurationLayoutGroup.position, Quaternion.identity, confConfigurationLayoutGroup);
                    confItem.gameObject.SetActive(true);
                    var name = confItem.transform.GetChild(0).GetComponent<TMP_Text>();
                    var button = confItem.GetComponent<Button>();

                    name.text = item.Produkt;
                    button.onClick.AddListener(() =>
                    {
                        activeConfiguration = item;
                        Prompt.Instance.ShowLoadingBar();

                        confSpecificConfigurationPanel.gameObject.SetActive(true);
                        specificConfNameInput.text = item.Produkt;
                        specificConfPriceInput.text = activeConfiguration.Cena;

                        CallWebActionGetConfigurationMenuItems(activeConfiguration.ID_opcji);
                    });

                    spawnedConfigurationItems.Add(confItem);
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }

    async void CallWebActionGetConfigurationMenuItems(string id)
    {
        if (spawnedItemToConfigure.Count > 0) spawnedItemToConfigure.ForEach(item => { Destroy(item.gameObject); });
        spawnedItemToConfigure.Clear();

        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetConfigurationMenuItemsOracle(id, cancellationToken, (text) =>
                {
                    tcs.SetResult(text);
                });

                string responseText;
                try
                {
                    responseText = await tcs.Task;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Operation was cancelled.");
                    return; // Early exit if the operation is cancelled
                }

                if (responseText.Contains("error"))
                {
                    Prompt.Instance.ShowTooltip(responseText);
                    return;
                }

                ConfigurationMenuItemsJson item = JsonConvert.DeserializeObject<ConfigurationMenuItemsJson>(responseText);
                
                foreach (var related in item.related)
                {
                    var spawnedItem = Instantiate(itemToConfigure, specificConfLayoutGroup.position, Quaternion.identity, specificConfLayoutGroup.transform);
                    spawnedItem.gameObject.SetActive(true);

                    var text = spawnedItem.transform.Find("Text").GetComponent<TMP_Text>();
                    var button = spawnedItem.GetComponent<Button>();
                    var checkmark = spawnedItem.transform.Find("CheckmarkBackground").GetChild(0);

                    var createdItem = spawnedItem.AddComponent<ItemToConfigure>();
                    createdItem.Init(new MenuItemIdName(related.ID_pozycji, related.Nazwaproduktu), text, checkmark, button, true);

                    spawnedItemToConfigure.Add(createdItem);
                }

                foreach (var unrelated in item.unrelated)
                {
                    var spawnedItem = Instantiate(itemToConfigure, specificConfLayoutGroup.position, Quaternion.identity, specificConfLayoutGroup.transform);
                    spawnedItem.gameObject.SetActive(true);

                    var text = spawnedItem.transform.Find("Text").GetComponent<TMP_Text>();
                    var button = spawnedItem.GetComponent<Button>();
                    var checkmark = spawnedItem.transform.Find("CheckmarkBackground").GetChild(0);

                    var createdItem = spawnedItem.AddComponent<ItemToConfigure>();
                    createdItem.Init(new MenuItemIdName(unrelated.ID_pozycji, unrelated.Nazwaproduktu), text, checkmark, button);

                    spawnedItemToConfigure.Add(createdItem);
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }

    async void CallWebActionDeleteConfiguration(string idOpcji)
    {
        if (spawnedItemToConfigure.Count > 0) spawnedItemToConfigure.ForEach(item => { Destroy(item.gameObject); });
        spawnedItemToConfigure.Clear();

        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.DeleteConfigurationOracle(idOpcji, cancellationToken, (text) =>
                {
                    tcs.SetResult(text);
                });

                string responseText;
                try
                {
                    responseText = await tcs.Task;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Operation was cancelled.");
                    return; // Early exit if the operation is cancelled
                }

                if (responseText.Contains("error"))
                {
                    Prompt.Instance.ShowTooltip(responseText);
                    return;
                }

                Prompt.Instance.ShowTooltip(responseText, () =>
                {
                    confSpecificConfigurationPanel.gameObject.SetActive(false);
                    ConfConfigurationInit();
                });
            }
        });
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

    public void ConfConfigurationInit()
    {
        if (spawnedConfigurationItems.Count > 0) spawnedConfigurationItems.ForEach(item => { Destroy(item.gameObject); });
        spawnedConfigurationItems.Clear();

        Prompt.Instance.ShowLoadingBar();
        if (!taskManager.IsTaskRunning) CallWebActionGetConfiguration();
    }

    public void DeleteConf()
    {
        Prompt.Instance.ShowLoadingBar();
        CallWebActionDeleteConfiguration(activeConfiguration.ID_opcji);
    }

    public void UpdateConf()
    {

    }
}

public class MenuItemIdName
{
    public string ID_pozycji { get; set; }
    [JsonProperty("Nazwa produktu")]
    public string NazwaProduktu { get; set; }
    public MenuItemIdName(string iD_pozycji, string nazwaproduktu)
    {
        ID_pozycji = iD_pozycji;
        NazwaProduktu = nazwaproduktu;
    }
}

public class ItemToConfigure : MonoBehaviour
{
    public MenuItemIdName Data;
    public Transform Checkmark;
    public bool ActiveCheckmark = false;

    public void Init(MenuItemIdName data, TMP_Text text, Transform checkmark, Button button, bool activeCheckmark = false)
    {
        Data = data;
        Checkmark = checkmark;
        ActiveCheckmark = activeCheckmark;

        text.text = $"{Data.ID_pozycji}:{Data.NazwaProduktu}";
        checkmark.gameObject.SetActive(ActiveCheckmark);
        button.onClick.AddListener(() =>
        {
            ActiveCheckmark = !ActiveCheckmark;
            checkmark.gameObject.SetActive(ActiveCheckmark);
        });

    }

    public bool GetActiveCheckmark()
    {
        if (Checkmark.gameObject.activeSelf) return true;
        return false;
    }

    public void TurnCheckmarksOff()
    {
        Checkmark.gameObject.SetActive(false);
        ActiveCheckmark = false;
    }
}

[Serializable]
public class ConfigurationItemOnlyJson
{
    public string ID_opcji { get; set; }
    public string Produkt { get; set; }
    public string Cena { get; set; }
}


public class ConfigurationMenuItemsJson
{
    public Related[] related { get; set; }
    public Unrelated[] unrelated { get; set; }
}

public class Related
{
    public string ID_pozycji { get; set; }
    [JsonProperty("Nazwa produktu")]
    public string Nazwaproduktu { get; set; }
}

public class Unrelated
{
    public string ID_pozycji { get; set; }
    [JsonProperty("Nazwa produktu")]
    public string Nazwaproduktu { get; set; }
}
