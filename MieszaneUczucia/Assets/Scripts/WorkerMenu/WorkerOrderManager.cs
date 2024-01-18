using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorkerOrderManager : MonoBehaviour
{
    [Header("OrdersPanel")]
    [SerializeField] Transform ordersMenuPanel;
    [SerializeField] Transform ordersLayoutGroup;
    [SerializeField] GameObject ordersItem;
    List<GameObject> spawnedOrders = new List<GameObject>();
    OrderJson activeOrder;

    [Header("DetailedPanel")]
    [SerializeField] Transform detailedPanel;
    [SerializeField] Transform detailedOrderItem;
    [SerializeField] Transform detailedLayoutGroup;
    [SerializeField] TMP_Dropdown statusDropdown;
    List<Transform> spawnedDetailed = new List<Transform>();

    TaskManager taskManager = new TaskManager();

    public void ShowOrders()
    {
        if(spawnedOrders.Count > 0)
        {
            spawnedOrders.ForEach(item => Destroy(item));
            spawnedOrders.Clear();
        }

        Prompt.Instance.ShowLoadingBar();
        CallWebGetOrders();
    }

    public void UpdateStaus()
    {
        Prompt.Instance.ShowLoadingBar();
        CallWebUpdateStatus();
    }

    public void Home()
    {
        SceneManager.LoadScene("Menu");
    }

    async void CallWebGetOrders()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetOrderOracle(cancellationToken, (text) =>
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
                    Prompt.Instance.ShowTooltip(responseText, () =>
                    {
                        ordersMenuPanel.gameObject.SetActive(false);
                    });
                    return;
                }

                List<OrderJson> orders = JsonConvert.DeserializeObject<List<OrderJson>>(responseText);

                foreach (var order in orders)
                {
                    var item = Instantiate(ordersItem, ordersLayoutGroup.position, Quaternion.identity, ordersLayoutGroup);
                    item.gameObject.SetActive(true);
                    var info = item.transform.Find("Info").GetComponent<TMP_Text>();
                    var button = item.GetComponent<Button>();

                    string formattedDescription;
                    if (order.Opis.Contains("Adres"))
                    {
                        formattedDescription = order.Opis
                                .Replace("Dostawa: ", "\nDostawa: ")
                                .Replace(", Adres: ", "\nAdres: ")
                                .Replace(". Zamówienie", "\nZamówienie");
                    }
                    else
                    {
                        formattedDescription = order.Opis
                                .Replace("Dostawa: ", "\nDostawa: ")
                                .Replace(". Zamówienie", "\nZamówienie");
                    }
                    

                    info.text = $"Id: {order.ID_zamowienia} \n" +
                                $"Status: {order.GetColoredStatus()} \n" +
                                $"Data: {order.Data} \n" +
                                $"{formattedDescription}";
                    button.onClick.AddListener(() =>
                    {
                        activeOrder = order;
                        ShowOrderDetails(order);
                    });

                    spawnedOrders.Add(item);
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }

    async void CallWebUpdateStatus()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.UpdateStatucOracle(activeOrder.ID_zamowienia, statusDropdown.options[statusDropdown.value].text, cancellationToken, (text) =>
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
                    Prompt.Instance.ShowTooltip(responseText, () =>
                    {
                        detailedPanel.gameObject.SetActive(false);
                    });
                    return;
                }

                Prompt.Instance.ShowTooltip(responseText, () =>
                {
                    ShowOrders();
                    detailedPanel.gameObject.SetActive(false);
                    Notifications.SendCustomNotification("Status zamówienia", $"Zmieniono na {statusDropdown.options[statusDropdown.value].text}", activeOrder.ID_klienta);
                });
            }
        });
    }

    void ShowOrderDetails(OrderJson orderData)
    {
        if(spawnedDetailed.Count > 0)
        {
            spawnedDetailed.ForEach(item => Destroy(item.gameObject));
            spawnedDetailed.Clear();
        }
        detailedPanel.gameObject.SetActive(true);
        statusDropdown.value = orderData.SetDropDownStatus();

        var orderValues = JsonConvert.DeserializeObject<List<Order>>(orderData.Zamowione);

        foreach (var order in orderValues)
        {
            var basketItem = Instantiate(detailedOrderItem, detailedLayoutGroup.position, Quaternion.identity, detailedLayoutGroup);
            basketItem.gameObject.SetActive(true);
            var name = basketItem.GetChild(0).GetComponent<TMP_Text>();
            int itemSize = 200;
            int selectedConfItems = 0;

            name.text = $"<b>{order.MenuItem.Name} x{order.MenuItem.Amount}: </b>\n";

            foreach (var conf in order.ConfigurationItems)
            {
                if (conf.Amount <= 0) continue;
                name.text += $"<indent=5%>-<color=#616266>{conf.Name} x{conf.Amount}: </color></indent>\n";
                itemSize += 100;
                selectedConfItems += 1;
            }

            var rect = basketItem.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, selectedConfItems > 0 ? itemSize - 50 : itemSize);
            
            spawnedDetailed.Add(basketItem);
        }
    }
}

public class OrderJson
{
    public string ID_zamowienia { get; set; }
    public string ID_klienta { get; set; }
    public string Status { get; set; }
    public string Data { get; set; }
    public string Zamowione { get; set; }
    public string Opis { get; set; }

    public string GetColoredStatus()
    {
        if (Status.Contains("oczekuje")) return $"<color=#db503d>{Status}</color>";
        if (Status.Contains("potwierdzone")) return $"<color=#dbb63d>{Status}</color>";
        if (Status.Contains("w_trakcie")) return $"<color=#b3721d>{Status}</color>";
        if (Status.Contains("gotowe_do_odbioru")) return $"<color=#4fdb3d>{Status}</color>";
        if (Status.Contains("zakoncone")) return $"<color=#db3d65>{Status}</color>";

        return Status;
    }

    public int SetDropDownStatus()
    {
        if (Status.Contains("oczekuje")) return 0;
        if (Status.Contains("potwierdzone")) return 1;
        if (Status.Contains("w_trakcie")) return 2;
        if (Status.Contains("gotowe_do_odbioru")) return 3;
        if (Status.Contains("zakoncone")) return 4;

        return 0;
    }
}



