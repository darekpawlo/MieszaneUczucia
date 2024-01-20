using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{   
    [SerializeField] Transform orderPanel;
    [SerializeField] Transform selectedItem;
    [SerializeField] ConfigurationItem configurationItemPrefab;
    [SerializeField] Transform layoutGroup;    

    List<ConfigurationItem> spawnedConfigurationItems = new List<ConfigurationItem>();

    [Header("BottomBar")]
    [SerializeField] TMP_Text priceText;
    [SerializeField] TMP_Text amountText;
    MenuItemData activeItem;
    int menuItemAmount = 1;
    public static float orderPrice = 0;

    [Header("LoadingBar")]
    [SerializeField] Transform loadingBarPrefab;
    Transform activeBar;
    float barValue = 0;

    TaskManager taskManager = new TaskManager();

    private void Start()
    {
        OrderPanelState(false);
    }

    private void Update()
    {
        if (activeBar == null) return;

        barValue += 0.01f;
        UpdateLoadingBar(barValue);
        if(barValue >= 1) 
        {
            barValue = 0;   
        }
    }

    public void Init(MenuItemData data)
    {
        activeItem = data;
        
        foreach (var item in spawnedConfigurationItems)
        {
            Destroy(item.gameObject);
        }
        spawnedConfigurationItems.Clear();

        orderPrice = 0;
        menuItemAmount = 1;
        orderPrice += activeItem.Price;

        SetOrderValues(data);
        UpdatePriceText();

        CallWebAction(data);

        SpawnBar();
    }

    async void CallWebAction(MenuItemData data)
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetOrderConfigurationOracleAsync(data.Id, cancellationToken, (text) =>
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

                List<ConfigurationItemData> configurationItemsData = JsonConvert.DeserializeObject<List<ConfigurationItemData>>(responseText);
                foreach (var configurationItemData in configurationItemsData)
                {
                    var item = Instantiate(configurationItemPrefab, configurationItemPrefab.transform.position, Quaternion.identity, layoutGroup);
                    item.Init(configurationItemData, UpdatePriceText);

                    spawnedConfigurationItems.Add(item);
                }
            }
        });

        DestoryBar();
    }


    public void SetOrderValues(MenuItemData data)
    {
        selectedItem.Find("Icon").GetComponent<Image>().sprite = data.Icon;
        selectedItem.Find("Name").GetComponent<TMP_Text>().SetText(data.Name);
        selectedItem.Find("Price").GetComponent<TMP_Text>().SetText(data.Price.ToString("F"));
        selectedItem.Find("Description").GetComponent<TMP_Text>().SetText(data.Description);
    }

    public void UpdatePriceText()
    {
        priceText.text = orderPrice.ToString("F");
        amountText.text = $"{menuItemAmount}";
    }

    public void Confirm()
    {
        var menuItem = new MenuItemOrder(activeItem.Name, menuItemAmount, activeItem.Price);
        var configurationItems = new List<ConfigurationItemOrder>();

        foreach (var item in spawnedConfigurationItems)
        {
            if (item.amount <= 0) continue;
            var configurationItem = new ConfigurationItemOrder(item.data.Produkt, item.amount, item.data.Cena);
            configurationItems.Add(configurationItem);
        }

        BasketManager.Instance.AddOrder(new Order(orderPrice, menuItem, configurationItems));
        BasketManager.Instance.Init();

        taskManager.CancelCurrentTask();
    }

    public void MenuItemAmount(int amount)
    {
        if (menuItemAmount <= 1 && amount <= -1) return;

        menuItemAmount += amount;
        orderPrice += activeItem.Price * amount;
        UpdatePriceText();
    }

    public void Back()
    {
        taskManager.CancelCurrentTask();
        DestoryBar();  
    }

    public void OrderPanelState(bool state) => orderPanel.gameObject.SetActive(state);

    public Transform GetOrderPanel => orderPanel;
    
    void SpawnBar()
    {
        if(activeBar != null) Destroy(activeBar.gameObject);
        activeBar = Instantiate(loadingBarPrefab, loadingBarPrefab.transform.position, Quaternion.identity, layoutGroup);

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
}


[Serializable]
public struct ConfigurationItemData
{
    public string Produkt;
    public float Cena { get => cena/100; set => cena = value; }
    public int Zablokowane;
    float cena;
}