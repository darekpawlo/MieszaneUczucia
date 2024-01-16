using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class BlockMenuManager : MonoBehaviour
{
    [SerializeField] Transform blockMenuPanel;
    [SerializeField] Transform blockMenuLayoutGroup;
    [SerializeField] Transform blockMenuItem;
    List<(GameObject gameObject, string id)> spawnBlockMenuItems = new List<(GameObject, string)>();

    TaskManager taskManager = new TaskManager();

    public void RefreshMenu()
    {
        Prompt.Instance.ShowLoadingBar();
        if (spawnBlockMenuItems.Count > 0)
        {
            spawnBlockMenuItems.ForEach(item => { Destroy(item.gameObject); });
            spawnBlockMenuItems.Clear();
        }

        CallWebGetMenu();
    }

    public void UpdateMenu()
    {
        string idsToBlock = "";
        string idsToUnblock = "";
        foreach (var item in spawnBlockMenuItems)
        {
            var blocked = item.gameObject.transform.Find("Blocked");
            if (blocked.gameObject.activeInHierarchy)
            {
                idsToBlock += $"{item.id},";
            }
            else
            {
                idsToUnblock += $"{item.id},";
            }
        }

        Prompt.Instance.ShowLoadingBar();
        CallWebUpdateBlockMenu(idsToBlock, idsToUnblock);
    }

    public void Back()
    {
        taskManager.CancelCurrentTask();
        blockMenuPanel.gameObject.SetActive(false);
    }

    public void BlockMenuInit()
    {
        Prompt.Instance.ShowLoadingBar();
        if (spawnBlockMenuItems.Count > 0)
        {
            spawnBlockMenuItems.ForEach(item => { Destroy(item.gameObject); });
            spawnBlockMenuItems.Clear();
        }

        CallWebGetMenu();
        blockMenuPanel.gameObject.SetActive(true);
    }

    async void CallWebGetMenu()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.GetMenu_Id_Name_Blocked_IconOracle(cancellationToken, (text) =>
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
                        blockMenuPanel.gameObject.SetActive(false);
                    });
                    return;
                }

                var menuItems = JsonConvert.DeserializeObject<List<Menu_Id_Name_Blocked_IconJson>>(responseText);
                foreach (var item in menuItems)
                {
                    var menuItem = Instantiate(blockMenuItem, blockMenuLayoutGroup.position, Quaternion.identity, blockMenuLayoutGroup);
                    menuItem.gameObject.SetActive(true);
                    menuItem.Find("Name").GetComponent<TMP_Text>().text = $"Id:{item.ID_pozycji} \n{item.Nazwaproduktu}";
                    menuItem.Find("Icon").GetComponent<Image>().sprite = ImageManager.BytesToSprite(Convert.FromBase64String(item.Zdjecia));
                    var blocked = menuItem.Find("Blocked");
                    blocked.gameObject.SetActive(int.Parse(item.Zablokowane) == 1);
                    blocked.GetChild(0).GetComponent<TMP_Text>().text = "Zablokowane";

                    menuItem.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        blocked.gameObject.SetActive(!blocked.gameObject.activeInHierarchy);
                    });

                    spawnBlockMenuItems.Add((menuItem.gameObject, item.ID_pozycji));
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }

    async void CallWebUpdateBlockMenu(string idsToBlock, string idToUnblock)
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.UpdateBlockMenuOracle(idsToBlock, idToUnblock, cancellationToken, (text) =>
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
                        blockMenuPanel.gameObject.SetActive(false);
                    });
                    return;
                }

                Prompt.Instance.ShowTooltip(responseText, () =>
                {
                    RefreshMenu();
                });
            }
        });
    }
}

public class Menu_Id_Name_Blocked_IconJson
{
    public string ID_pozycji { get; set; }
    [JsonProperty("Nazwa produktu")]
    public string Nazwaproduktu { get; set; }
    public string Zablokowane { get; set; }
    public string Zdjecia { get; set; }
}
