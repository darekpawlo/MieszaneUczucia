using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasketManager : MonoBehaviour
{
    public static BasketManager Instance;
    public static float totalCost { get; private set; }

    [Header("Basket")]
    [SerializeField] Transform basket;
    [SerializeField] Transform basketPanel;
    [SerializeField] Transform basketParent;
    [SerializeField] Transform basketItemPrefab;

    [Header("BottomBar")]
    [SerializeField] Transform bottomBar;
    [SerializeField] Sprite loginIcon;
    [SerializeField] Sprite payIcon;

    [Header("Login")]
    [SerializeField] Transform loginPanel;

    [Header("Confirmation")]
    [SerializeField] Transform confirmationPanel;
    [SerializeField] TMP_Dropdown orderType;
    [SerializeField] TMP_InputField postalCode;
    [SerializeField] TMP_InputField city;
    [SerializeField] TMP_InputField road;
    [SerializeField] List<GameObject> clientAdressFields;

    [SerializeField] List<Order> orderds = new List<Order>();
    List<Transform> basketItems = new List<Transform>();

    TaskManager taskManager = new TaskManager();

    private void Awake()
    {
        Instance = this;    
    }

    public void Init()
    {
        basket.gameObject.SetActive(true);

        UpdateBasketText();
        UpdateBottomBar();
    }

    public void AddOrder(Order order)
    {
        orderds.Add(order);
        CalculateCost();
    }

    public void HideBasket()
    {
        basket.gameObject.SetActive(false);
    }

    public void CancelOrder()
    {
        foreach (var item in basketItems)
        {
            Destroy(item.gameObject);
        }
        basketItems.Clear();
        orderds.Clear();

        HideBasket();
        basketPanel.gameObject.SetActive(false);
    }

    public void UpdateBasketText()
    {
        CalculateCost();

        var ordersAmountText = basket.Find("BasketAmountText").GetComponent<TMP_Text>();
        var totalCost = basket.Find("TotalText").GetComponent<TMP_Text>();

        ordersAmountText.text = $"KOSZYK({orderds.Count})";
        totalCost.text = BasketManager.totalCost.ToString("F");
    }

    public void UpdateBottomBar()
    {
        CalculateCost();

        var text = bottomBar.Find("Text").GetComponent<TMP_Text>();
        var totalCost = bottomBar.Find("TotalText").GetComponent<TMP_Text>();
        var icon = bottomBar.Find("Image").GetChild(0).GetComponent<Image>();

        if (WebActions.UserInfo.ClientLoggedIn)
        {
            text.text = $"ZAMAWIAM I P£ACÊ: ";
            totalCost.text = BasketManager.totalCost.ToString("F");
            icon.sprite = payIcon;
        }
        else
        {
            text.text = $"ZALOGUJ SIÊ";
            totalCost.text = "";
            icon.sprite = loginIcon;
        }
    }


    public void ShowBasketItems()
    {
        ///////Cleanup//////////////
        foreach (var item in basketItems)
        {
            Destroy(item.gameObject);
        }
        basketItems.Clear();
        ////////////////////////////

        basketPanel.gameObject.SetActive(true);

        foreach (var order in orderds)
        {
            var basketItem = Instantiate(basketItemPrefab, basketParent.transform.position, Quaternion.identity, basketParent);
            var name = basketItem.GetChild(0).GetComponent<TMP_Text>();
            var price = basketItem.GetChild(1).GetComponent<TMP_Text>();
            Button cancel = basketItem.GetChild(2).GetComponent<Button>();

            float orderPrice = order.MenuItem.Amount * order.MenuItem.Price;
            int itemSize = 200;
            int selectedConfItems = 0;

            name.text = $"<b>{order.MenuItem.Name} x{order.MenuItem.Amount}: </b>\n";
            price.text =  $"<b>{orderPrice:F} </b>\n";

            foreach (var conf in order.ConfigurationItems)
            {
                if (conf.Amount <= 0) continue;
                name.text += $"<indent=5%>-<color=#616266>{conf.Name} x{conf.Amount}: </color></indent>\n";
                price.text += $"<color=#616266>{conf.Amount * conf.Price:F}</color>\n";
                itemSize += 100;
                selectedConfItems += 1;
            }

            if(selectedConfItems > 0)
            {
                name.text += $"<b>TOTAL: </b>";
                price.text += $"<b>{order.CalculatePrice():F}</b>";
                itemSize += 100;
            }

            var rect = basketItem.GetComponent<RectTransform>();            
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, selectedConfItems > 0 ? itemSize - 50 : itemSize);

            cancel.onClick.AddListener(() =>
            {
                Destroy(basketItem.gameObject);
                basketItems.Remove(basketItem);
                orderds.Remove(order);

                ShowBasketItems();
                UpdateBottomBar();
            });

            basketItems.Add(basketItem);
        }
    }

    public void Back()
    {
        if(totalCost <= 0) HideBasket();
        UpdateBasketText();
    }

    public void Confirm()
    {
        if(WebActions.UserInfo.ClientLoggedIn)
        {
            ConfirmationPanelInit();
        }
        else
        {
            loginPanel.gameObject.SetActive(true);
        }
    }

    public void CancelConfirmation()
    {
        orderType.value = 0;
        confirmationPanel.gameObject.SetActive(false);
        CancelOrder();
    }

    public void ConfirmConfirmation()
    {
        if(orderType.value == 1)
        {
            if (postalCode.text.Length < 0 || postalCode.text.Length > 5)
            {
                Prompt.Instance.ShowTooltip("Nie poprawny kod pocztowy");
                return;
            }

            PlayerPrefs.SetInt("KodPocztowy", int.Parse(postalCode.text));
            PlayerPrefs.SetString("Miasto", city.text);
            PlayerPrefs.SetString("Ulica", road.text);

            if (int.Parse(postalCode.text) != 59600)
            {
                Prompt.Instance.ShowTooltip("Przepraszamy, ale ten rejon nie jest jeszcze obs³ugiwany przez nasz¹ restauracjê!");
                return;
            }
        }      

        if(taskManager.IsTaskRunning)
        {
            Debug.Log("Task is already running!");
            Prompt.Instance.ShowTooltip("Akcja jest ju¿ wykonywana!");
            return;
        }

        Prompt.Instance.ShowLoadingBar();
        CallWebAction();
    }

    void CalculateCost()
    {
        totalCost = 0;
        foreach (var order in orderds)
        {
            totalCost += order.CalculatePrice();
        }
    }

    void ConfirmationPanelInit()
    {
        confirmationPanel.gameObject.SetActive(true);

        if(PlayerPrefs.HasKey("KodPocztowy"))
        {
            postalCode.text = PlayerPrefs.GetInt("KodPocztowy").ToString();
            city.text = PlayerPrefs.GetString("Miasto").ToString();
            road.text = PlayerPrefs.GetString("Ulica").ToString();
        }        

        orderType.onValueChanged.AddListener((value) =>
        {
            switch (value)
            {
                case 0:
                    clientAdressFields.ForEach(gameObject => gameObject.SetActive(false));
                    break;
                case 1:
                    clientAdressFields.ForEach(gameObject => gameObject.SetActive(true));
                    break;
            }
        });
    }

    async void CallWebAction()
    {
        string opisText = string.Empty;
        if (orderType.value == 0)
        {
            opisText = $"Dostawa: {orderType.options[orderType.value].text}. ";
        }
        else
        {
            opisText = $"Dostawa: {orderType.options[orderType.value].text}, Adres: {postalCode.text}, {city.text}, {road.text}. ";
        }
        opisText += $"Zamówienie kosztowa³o: { totalCost}";

        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.InsertZamowienieOracleAsync(WebActions.UserInfo.ID, "oczekuje", FormatOrderToString(), opisText, cancellationToken, (text) =>
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
                    return;
                }

                Prompt.Instance.ShowTooltip("Zamówienie oczekuje akceptacji przez obs³ugê!", () =>
                {
                    confirmationPanel.gameObject.SetActive(false);
                    CancelOrder();
                });
            }
        });
    }    

    string FormatOrderToString()
    {
        return JsonConvert.SerializeObject(orderds, Formatting.Indented);
    }
}
