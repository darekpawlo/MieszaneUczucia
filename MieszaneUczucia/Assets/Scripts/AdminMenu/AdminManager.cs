using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
    List<Transform> spawnedWorkers = new List<Transform>();
    Employee activeWorker;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowAccounts()
    {
        AddAccountPanel.gameObject.SetActive(false);
        ConfigureAccountsPanel.gameObject.SetActive(true);

        StartCoroutine(WebActions.AllWorkersOracle((jsonString) =>
        {
            foreach (var workerObject in spawnedWorkers)
            {
                Destroy(workerObject.gameObject);
            }
            spawnedWorkers.Clear();

            var workersList = JsonConvert.DeserializeObject<List<Employee>>(jsonString);
            foreach (var worker in workersList)
            {
                var workerPrefab = workerLayoutGroup.GetChild(0);
                var workerObject = Instantiate(workerPrefab, workerPrefab.transform.position, Quaternion.identity, workerLayoutGroup);
                var text = workerObject.GetChild(0).GetComponent<TMP_Text>();
                workerObject.gameObject.SetActive(true);

                text.SetText($"" +
                    $"ID: {worker.Id}, Type: {worker.Type} \n" +
                    $"Name: {worker.Name} \n" +
                    $"Password: {worker.Password} \n" +                 
                    $"");
                workerObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    nameInput.text = worker.Name;
                    passwordInput.text = worker.Password;
                    typeInput.text = worker.Type;

                    activeWorker = worker;
                    configurationPanel.gameObject.SetActive(true);
                });

                spawnedWorkers.Add(workerObject);
            }
        }));        
    }

    public void UpdateWorker()
    {
        StartCoroutine(WebActions.UpdateWorkerOracle(activeWorker.Id, nameInput.text, passwordInput.text, typeInput.text, (webText) =>
        {
            configurationPanel.gameObject.SetActive(false);
            ShowAccounts();
        }));
    }

    public void DeleteWorker()
    {
        StartCoroutine(WebActions.DeleteWorkerOracle(activeWorker.Id, (webText) =>
        {
            configurationPanel.gameObject.SetActive(false);
            ShowAccounts();
        }));
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
                return;
            }

            StartCoroutine(WebActions.RegisterWorkerOracle(loginInput.text, passInput.text, typeDropdown.options[typeDropdown.value].text));
        });
    }

    public void Back()
    {
        AddAccountPanel.gameObject.SetActive(false);
        ConfigureAccountsPanel.gameObject.SetActive(false);
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
