using System;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class NotificationsPhp : MonoBehaviour
{
    public static NotificationsPhp Instance { get; private set; }

    List<OrderStatus> activeStatus= new List<OrderStatus>();

    TaskManager taskManager = new TaskManager();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(NotificationCoroutine());
    }

    public string GetColoredStatus(string Status)
    {
        if (Status.Contains("oczekuje")) return $"<color=#db503d>{Status}</color>";
        if (Status.Contains("potwierdzone")) return $"<color=#dbb63d>{Status}</color>";
        if (Status.Contains("w_trakcie")) return $"<color=#b3721d>{Status}</color>";
        if (Status.Contains("gotowe_do_odbioru")) return $"<color=#4fdb3d>{Status}</color>";
        if (Status.Contains("zakoncone")) return $"<color=#db3d65>{Status}</color>";

        return Status;
    }

    private IEnumerator NotificationCoroutine()
    {
        while (true)
        {
            ShowNotification();
            yield return new WaitForSeconds(15);
        }
    }

    private void ShowNotification()
    {
        if (PlayerPrefs.HasKey("ID_klienta") && activeStatus != null)
        {
            CallWebUpdateStatus();
        }
    }

    async void CallWebUpdateStatus()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetOrdersStatusOracle(PlayerPrefs.GetString("ID_klienta"), cancellationToken, (text) =>
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

                var orders = JsonConvert.DeserializeObject<List<OrderStatus>>(responseText);

                foreach (var status in orders)
                {
                    foreach (var activeStatus in activeStatus)
                    {
                        if(status.ID_zamowienia == activeStatus.ID_zamowienia && status.Status != activeStatus.Status)
                        {
                            Debug.Log($"Status siê zmieni³ z {activeStatus.Status} na {status.Status}");
                            Prompt.Instance.ShowTooltip($"Status zamówienia {status.ID_zamowienia} zosta³ zmieniony na: {GetColoredStatus(status.Status)}");
                        }
                    }
                }

                activeStatus = orders;
            }
        });
    }
}

public class OrderStatus
{
    public string ID_zamowienia { get; set; }
    public string ID_klienta { get; set; }
    public string Status { get; set; }
    public string Data { get; set; }
    public string Zamowione { get; set; }
    public string Opis { get; set; }
}
