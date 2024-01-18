using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class WarehourseManager : MonoBehaviour
{
    [Header("Showing warehouse")]
    [SerializeField] Transform warehousePanel;
    [SerializeField] Transform warehouseObject;
    [SerializeField] Transform warehouseLayout;
    List<Transform> spawnedWarehouseObjects = new List<Transform>();

    [Header("Adding warehouse")]
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField amountInput;

    [Header("Updating warehouse")]
    List<(string id, TMP_InputField amount)> amountInputFields = new List<(string, TMP_InputField)>();

    TaskManager taskManager = new TaskManager();

    public void UpdateWarehouse()
    {
        Prompt.Instance.ShowLoadingBar();
        CallWebUpdateWarehouse();
    }

    public void AddWarehouseRecord()
    {
        if(nameInput.text == string.Empty)
        {
            Prompt.Instance.ShowTooltip("Wype³nij wszystkie pola!");
            return;
        }
        if (amountInput.text == string.Empty)
        {
            Prompt.Instance.ShowTooltip("Wype³nij wszystkie pola!");
            return;
        }
        Prompt.Instance.ShowLoadingBar();
        CallWebInsertWarehouse();
    }    

    public void WarehouseInit()
    {
        if(spawnedWarehouseObjects.Count > 0)
        {
            spawnedWarehouseObjects.ForEach(item => { Destroy(item.gameObject); }); 
            spawnedWarehouseObjects.Clear();
        }

        Prompt.Instance.ShowLoadingBar();
        warehousePanel.gameObject.SetActive(true);
        CallWebGetWarehouse();
    }

    async void CallWebGetWarehouse()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetWarehouseOracle(cancellationToken, (text) =>
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
                        warehousePanel.gameObject.SetActive(false);
                    });
                    return;
                }

                var warehouseJsons = JsonConvert.DeserializeObject<List<WarehouseJson>>(responseText);

                foreach (var item in warehouseJsons)
                {
                    var record = Instantiate(warehouseObject, warehouseLayout.position, Quaternion.identity, warehouseLayout);
                    record.gameObject.SetActive(true);
                    var info = record.Find("Info").GetComponent<TMP_Text>();
                    var cancel = record.Find("Cancel").GetComponent<Button>();
                    var amount = record.Find("InputField").GetComponent<TMP_InputField>();

                    info.text = $"Id: {item.ID_produktu} \n" +
                                $"Nazwa: {item.Nazwa} \n" +
                                $"Iloœæ:";
                    amount.text = $"{item.Ilosc}";
                    cancel.onClick.AddListener(() =>
                    {
                        DeleteWarehouseRecord(item.ID_produktu);
                    });

                    spawnedWarehouseObjects.Add(record);
                    amountInputFields.Add((id: item.ID_produktu, amount: amount));
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }

    async void CallWebDeleteWarehouse(string IdProduktu)
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.DeleteWarehouseOracle(IdProduktu, cancellationToken, (text) =>
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
                        warehousePanel.gameObject.SetActive(false);
                    });
                    return;
                }

                Prompt.Instance.ShowTooltip(responseText, () =>
                {
                    WarehouseInit();
                });
            }
        });
    }

    async void CallWebInsertWarehouse()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.InsertWarehouseOracle(nameInput.text, amountInput.text, cancellationToken, (text) =>
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
                        warehousePanel.gameObject.SetActive(false);
                        nameInput.text = "";
                        amountInput.text = "";
                    });
                    return;
                }

                Prompt.Instance.ShowTooltip(responseText, () =>
                {
                    WarehouseInit();
                    nameInput.text = "";
                    amountInput.text = "";
                });
            }
        });
    }

    async void CallWebUpdateWarehouse()
    {
        string IdPozycji = "";
        string Ilosci = "";
        foreach (var item in amountInputFields)
        {
            IdPozycji += $"{item.id},";
            Ilosci += $"{item.amount.text},";
        }

        Debug.Log(IdPozycji);
        Debug.Log(Ilosci);

        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.UpdateWarehouseOracle(IdPozycji, Ilosci, cancellationToken, (text) =>
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
                        warehousePanel.gameObject.SetActive(false);
                    });
                    return;
                }

                Prompt.Instance.ShowTooltip(responseText, () =>
                {
                    WarehouseInit();
                });
            }
        });
    }

    void DeleteWarehouseRecord(string IdProduktu)
    {
        Prompt.Instance.ShowLoadingBar();
        CallWebDeleteWarehouse(IdProduktu);
    }
}

public class WarehouseJson
{
    public string ID_produktu { get; set; }
    public string Nazwa { get; set; }
    public string Ilosc { get; set; }
}
