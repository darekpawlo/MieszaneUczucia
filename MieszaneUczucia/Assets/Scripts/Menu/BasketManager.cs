using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasketManager : MonoBehaviour
{
    public static BasketManager Instance;
    public static float totalCost { get; private set; }

    [SerializeField] Transform basket;
    [SerializeField] Transform bottomBar;
    [SerializeField] Transform basketPanel;
    [SerializeField] Transform basketParent;
    [SerializeField] Transform basketItemPrefab;
    [SerializeField] List<Order> orderds = new List<Order>();

    List<Transform> basketItems = new List<Transform>();

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

        text.text = $"ZAP£AÆ: ";
        totalCost.text = BasketManager.totalCost.ToString("F");
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

            float orderPrice = order.MenuItem.Amount * order.MenuItem.Data.Price;
            int itemSize = 200;
            int selectedConfItems = 0;

            name.text = $"<b>{order.MenuItem.Data.Name} x{order.MenuItem.Amount}: </b>\n";
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
                //UpdateBasketText();
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

    }

    void CalculateCost()
    {
        totalCost = 0;
        foreach (var order in orderds)
        {
            totalCost += order.CalculatePrice();
        }
    }
}
