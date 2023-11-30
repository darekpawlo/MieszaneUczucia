using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdminManager : MonoBehaviour
{
    public static AdminManager Instance;

    [SerializeField] private Transform AddAccountPanel;
    [SerializeField] private Transform ConfigureAccountsPanel;

    [Header("CrateAccount")]
    [SerializeField] private Button CreateAccountButton;
    [SerializeField] private TMP_InputField loginInput;
    [SerializeField] private TMP_InputField passInput;
    [SerializeField] private TMP_Dropdown typeDropdown;

    private void Awake()
    {
        Instance = this;
    }

    public void ConfigureAccounts()
    {
        AddAccountPanel.gameObject.SetActive(false);
        ConfigureAccountsPanel.gameObject.SetActive(true);

        StartCoroutine(WebActions.AllUsersOracle((jsonString) =>
        {
            var rootObject = JsonConvert.DeserializeObject<RootObject>(jsonString);
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
        public string EmployeeId { get; set; }

        [JsonProperty("Typ_pracownika")]
        public string EmployeeType { get; set; }

        [JsonProperty("Haslo")]
        public string Password { get; set; }

        [JsonProperty("Nazwa")]
        public string Name { get; set; }
    }
    public struct Client
    {
        [JsonProperty("ID_klienta")]
        public string ClientId { get; set; }

        [JsonProperty("Nazwa")]
        public string Name { get; set; }

        [JsonProperty("Nrtelefonu")]
        public string PhoneNumber { get; set; }

        [JsonProperty("Haslo")]
        public string Password { get; set; }
    }
    public struct RootObject
    {
        [JsonProperty("PRACOWNICY")]
        public List<Employee> Employees { get; set; }

        [JsonProperty("KLIENCI")]
        public List<Client> Clients { get; set; }
    }
}
