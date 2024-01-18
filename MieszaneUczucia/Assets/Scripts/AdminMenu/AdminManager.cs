using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading;
using System.Threading.Tasks;

public class AdminManager : MonoBehaviour
{
    public static AdminManager Instance;

    [SerializeField] Transform AddAccountPanel;
    [SerializeField] Transform ConfigureAccountsPanel;

    [Header("CrateAccount")]
    [SerializeField] Button CreateAccountButton;
    [SerializeField] TMP_InputField loginInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] TMP_Dropdown typeDropdown;

    [Header("ConfigureAccoutn")]
    [SerializeField] Transform workerLayoutGroup;
    [SerializeField] Transform configurationPanel;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_InputField typeInput;
    [SerializeField] TMP_Dropdown confTypeDropdown;
    List<Transform> spawnedWorkers = new List<Transform>();
    Employee activeWorker;

    TaskManager taskManager = new TaskManager();

    private void Awake()
    {
        Instance = this;
    }

    public void ShowAccounts()
    {
        AddAccountPanel.gameObject.SetActive(false);
        ConfigureAccountsPanel.gameObject.SetActive(true);

        foreach (var workerObject in spawnedWorkers)
        {
            Destroy(workerObject.gameObject);
        }
        spawnedWorkers.Clear();

        Prompt.Instance.ShowLoadingBar();
        CallWebGetWorkers();  
    }

    public void UpdateWorker()
    {
        Prompt.Instance.ShowLoadingBar();
        CallWebUpdateWorker();
    }

    public void DeleteWorker()
    {
        Prompt.Instance.ShowLoadingBar();
        CallWebDeleteWorker();
    }

    public void AddAccount()
    {
        AddAccountPanel.gameObject.SetActive(true);
        ConfigureAccountsPanel.gameObject.SetActive(false);

        CreateAccountButton.onClick.AddListener(() =>
        {
            if (loginInput.text == string.Empty || passInput.text == string.Empty)
            {
                Debug.Log("Fill all fields");
                Prompt.Instance.ShowTooltip("Uzupe³nij wszystkie pola!");
                return;
            }

            Prompt.Instance.ShowLoadingBar();
            CallWebRegisterWorker();
        });
    }

    public void Back()
    {
        AddAccountPanel.gameObject.SetActive(false);
        ConfigureAccountsPanel.gameObject.SetActive(false);
    }

    async void CallWebRegisterWorker()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.RegisterWorkerOracleAsync(loginInput.text, passInput.text, typeDropdown.options[typeDropdown.value].text, cancellationToken, (text) =>
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
                        //AddAccountPanel.gameObject.SetActive(false);
                        loginInput.text = "";
                        passInput.text = "";
                    });
                    return;
                }

                Prompt.Instance.ShowTooltip(responseText);
            }
        });
    }
    async void CallWebGetWorkers()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.AllWorkersOracleAsync(cancellationToken, (text) =>
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
                        
                    });
                    return;
                }

                var workersList = JsonConvert.DeserializeObject<List<Employee>>(responseText);
                foreach (var worker in workersList)
                {
                    var workerPrefab = workerLayoutGroup.GetChild(0);
                    var workerObject = Instantiate(workerPrefab, workerPrefab.transform.position, Quaternion.identity, workerLayoutGroup);
                    var text = workerObject.GetChild(0).GetComponent<TMP_Text>();
                    workerObject.gameObject.SetActive(true);

                    text.SetText($"" +
                        $"ID: {worker.Id}, Typ: {worker.Type} \n" +
                        $"Login: {worker.Name} \n" +
                        //$"Password: {worker.Password} \n" +                 
                        $"");
                    workerObject.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        nameInput.text = worker.Name;
                        passwordInput.text = worker.Password;
                        if(worker.Type.Contains("Pracownik"))
                        {
                            confTypeDropdown.value = 0;
                        }
                        else if(worker.Type.Contains("Menedzer"))
                        {
                            confTypeDropdown.value = 1;
                        }
                        else if(worker.Type.Contains("Admin"))
                        {
                            confTypeDropdown.value = 2;
                        }
                        //typeInput.text = worker.Type;

                        activeWorker = worker;
                        configurationPanel.gameObject.SetActive(true);
                    });

                    spawnedWorkers.Add(workerObject);
                }

                Prompt.Instance.HideLoadingBar();
            }
        });
    }
    async void CallWebUpdateWorker()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.UpdateWorkerOracleAsync(activeWorker.Id, nameInput.text, passwordInput.text, confTypeDropdown.options[confTypeDropdown.value].text, cancellationToken, (text) =>
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

                Prompt.Instance.ShowTooltip(responseText, () =>
                {
                    configurationPanel.gameObject.SetActive(false);
                    ShowAccounts();
                });
            }
        });
    }
    async void CallWebDeleteWorker()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();

            // Handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await WebActions.DeleteWorkerOracleAsync(activeWorker.Id, cancellationToken, (text) =>
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

                Prompt.Instance.ShowTooltip(responseText, () =>
                {
                    configurationPanel.gameObject.SetActive(false);
                    ShowAccounts();
                });
            }
        });
    }

    public struct Employee
    {
        [JsonProperty("ID_pracownika")]
        public string Id { get; set; }

        [JsonProperty("Typ_pracownika")]
        public string Type { get; set; }

        [JsonProperty("Haslo")]
        public string Password { get; set; }

        [JsonProperty("Nazwa")]
        public string Name { get; set; }
    }
}
