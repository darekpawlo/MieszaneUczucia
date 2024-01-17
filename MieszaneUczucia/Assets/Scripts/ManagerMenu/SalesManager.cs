using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SalesManager : MonoBehaviour
{
    [SerializeField] Transform salesPanel;
    [SerializeField] Transform salesLayoutGroup;
    [SerializeField] Transform salesItem;

    Dictionary<string, SalesObject> sales = new Dictionary<string, SalesObject>();

    TaskManager taskManager = new TaskManager();

    public void SalesInit()
    {
        if (sales.Count > 0)
        {
            foreach (var item in sales)
            {
                Destroy(item.Value.Transform.gameObject);
            }
            sales.Clear();
        }

        Prompt.Instance.ShowLoadingBar();
        salesPanel.gameObject.SetActive(true);
        CallWebGetSalesDone();
    }

    public void RefreshSales()
    {
        if (sales.Count > 0)
        {
            foreach (var item in sales)
            {
                Destroy(item.Value.Transform.gameObject);
            }
            sales.Clear();
        }

        Prompt.Instance.ShowLoadingBar();
        salesPanel.gameObject.SetActive(true);
        CallWebGetSalesDone();
    }

    async void CallWebGetSalesDone()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetOrdersDoneOracle(cancellationToken, (text) =>
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
                        salesPanel.gameObject.SetActive(false);
                    });
                    return;
                }

                List<OrderJson> orders = JsonConvert.DeserializeObject<List<OrderJson>>(responseText);
                foreach (var order in orders)
                {
                    var orderValues = JsonConvert.DeserializeObject<List<Order>>(order.Zamowione);
                    foreach (var value in orderValues)
                    {
                        //Populating directory
                        if(!sales.ContainsKey(value.MenuItem.Name))
                        {
                            sales.Add(value.MenuItem.Name, new SalesObject(Instantiate(salesItem, salesLayoutGroup.position, Quaternion.identity, salesLayoutGroup), value.MenuItem.Name, value.MenuItem.Amount, value.MenuItem.Price));
                        }
                        else if(sales.ContainsKey(value.MenuItem.Name))
                        {
                            sales[value.MenuItem.Name].IncreaseAmount(value.MenuItem.Amount);
                            sales[value.MenuItem.Name].UpdateValues();
                        }
                    }
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }
}

public class SalesObject
{
    public Transform Transform;
    public string Name;
    public int Amount;
    public float Price;

    public SalesObject(Transform transform, string name, int amount, float price)
    {
        Transform = transform;
        Name = name;
        Amount = amount;
        Price = price;

        transform.gameObject.SetActive(true);

        UpdateValues();
    }

    public void IncreaseAmount(int amount)
    {
        Amount += amount;
    }

    public float GetTotalPrice => Price * Amount;

    public void UpdateValues()
    {
        var info = Transform.Find("Info").GetComponent<TMP_Text>();

        info.text = $"Danie: {Name} \n" +
                    $"Zamówiono: {Amount} razy \n" +
                    $"Zarobi³o: {GetTotalPrice}";
    }
}
