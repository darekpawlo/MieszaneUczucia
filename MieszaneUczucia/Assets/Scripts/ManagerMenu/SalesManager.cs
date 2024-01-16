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
    List<Transform> spawnedSalesItems = new List<Transform>();

    TaskManager taskManager = new TaskManager();

    public void SalesInit()
    {
        if (spawnedSalesItems.Count > 0)
        {
            spawnedSalesItems.ForEach(item => { Destroy(item.gameObject); });
            spawnedSalesItems.Clear();
        }

        Prompt.Instance.ShowLoadingBar();
        salesPanel.gameObject.SetActive(true);
        CallWebGetSalesDone();
    }

    public void RefreshSales()
    {
        if (spawnedSalesItems.Count > 0)
        {
            spawnedSalesItems.ForEach(item => { Destroy(item.gameObject); });
            spawnedSalesItems.Clear();
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
                        Debug.Log(value.MenuItem.Name);
                    }

                    //var item = Instantiate(salesItem, salesLayoutGroup.position, Quaternion.identity, salesLayoutGroup);
                    //item.gameObject.SetActive(true);
                    //var info = item.transform.Find("Info").GetComponent<TMP_Text>();
                    //var button = item.GetComponent<Button>();


                    //spawnedSalesItems.Add(item);
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }
}
