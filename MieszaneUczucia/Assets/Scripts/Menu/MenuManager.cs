using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    public static List<MenuItemData> MenuItemsData { get; private set; } = new List<MenuItemData>();

    [SerializeField] MenuItem menuItemPrefab;
    [SerializeField] Transform menuItemHolder;
    [SerializeField] Transform menuPanel;
    

    [Header("Scripts")]
    [SerializeField] OrderManager orderManager;

    [Header("LoadingBar")]
    [SerializeField] Transform loadingBarPrefab;
    Transform activeBar;
    float barValue = 0;

    TaskManager taskManager = new TaskManager();

    private void Awake()
    {
        Instance = this;

        CallWebAction();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        menuPanel.gameObject.SetActive(true);

        SpawnBar();
    }

    private void Update()
    {
        if (activeBar == null) return;

        barValue += 0.01f;
        UpdateLoadingBar(barValue);
        if (barValue >= 1)
        {
            barValue = 0;
        }
    }

    public void GoToLogin()
    {
        Cancel();
        SceneManager.LoadScene("LoginMenu");
    }

    public void MenuPanelState(bool state) => menuPanel.gameObject.SetActive(state);

    async void CallWebAction()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            await WebActions.GetMenuOracleAsync(cancellationToken, (text) =>
            {
                tcs.SetResult(text);
            });

            string responseText = await tcs.Task;
            cancellationToken.ThrowIfCancellationRequested();

            var menuItems = JsonConvert.DeserializeObject<List<MenuItemJsonData>>(responseText);
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
                    orderManager.Init(itemData);
                });
            }
        });

        DestoryBar();
    }

    void SpawnBar()
    {
        if (activeBar != null) Destroy(activeBar.gameObject);
        activeBar = Instantiate(loadingBarPrefab, loadingBarPrefab.transform.position, Quaternion.identity, menuItemHolder);

        RectTransform rect = activeBar.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 850);
    }

    void DestoryBar()
    {
        if (activeBar == null) return;
        Destroy(activeBar.gameObject);
        activeBar = null;
    }

    void UpdateLoadingBar(float value)
    {
        Image bar = activeBar.Find("Bar").GetComponent<Image>();
        bar.fillAmount = value;
    }

    void Cancel()
    {
        taskManager.CancelCurrentTask();
    }
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

