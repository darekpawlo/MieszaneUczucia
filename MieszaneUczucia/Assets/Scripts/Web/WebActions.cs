using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;

public static class WebActions
{
    public static UserInfo UserInfo = new UserInfo();

    static string swiatolowod = "192.168.1.21";
    static string router = "192.168.8.108";

    public static string uzywaneIP = swiatolowod;
    static string baseUrl = $"http://{uzywaneIP}/UnityBackendTutorial/";

    static string menuOracle = $"{baseUrl}GetMenuOracle.php";
    static string loginOracle = $"{baseUrl}LoginOracle.php";
    static string registerOracle = $"{baseUrl}RegisterOracle.php";
    static string allUsersOracle = $"{baseUrl}GetAllAccounts.php";
    static string workersOracle = $"{baseUrl}GetWorkersOracle.php";
    static string registerWorkerOracle = $"{baseUrl}RegisterWorkerOracle.php";
    static string updateWorkerOracle = $"{baseUrl}UpdateWorkerOracle.php";
    static string deleteWorkerOracle = $"{baseUrl}DeleteWorkerOracle.php";    
    static string insertMenuOracle = $"{baseUrl}InsertMenuOracle.php";    
    static string updateMenuOracle = $"{baseUrl}UpdateMenuOracle.php";    
    static string deleteMenuOracle = $"{baseUrl}DeleteMenuOracle.php";    
    static string getOrderConfigurationOracle = $"{baseUrl}GetConfigurationOracle.php";        
    static string insertZamowienieOracle = $"{baseUrl}InsertZamowienieOracle.php";        
    static string insertKonfiguracjeOracle = $"{baseUrl}InsertConfigurationOracle.php";        
    static string getMenuOracle_Id_Name = $"{baseUrl}GetMenuOracle_Id_Name.php";        
    static string getOnlyConfigurationOracle = $"{baseUrl}GetOnlyConfigurationOracle.php";        
    static string insertOptionsConfigurationOracle = $"{baseUrl}InsertOptionsConfigurationOracle.php";        
    static string getConfigurationMenuItemsOracle = $"{baseUrl}GetConfigurationMenuItemsOracle.php";        
    static string deleteConfigurationOracle = $"{baseUrl}DeleteConfigurationOracle.php";        
    static string updateConfigurationOracle = $"{baseUrl}UpdateConfigurationOracle.php";        
    static string getOrdersOracle = $"{baseUrl}GetOrdersOracle.php";        
    static string updateStatusOracle = $"{baseUrl}UpdateStatusOracle.php";        
    static string getWarehouseOracle = $"{baseUrl}GetWarehouseOracle.php";        
    static string deleteWarehouseOracle = $"{baseUrl}DeleteWarehouseOracle.php";        
    static string insertWarehouseOracle = $"{baseUrl}InsertWarehouseOracle.php";        
    static string updateWarehouseOracle = $"{baseUrl}UpdateWarehouseOracle.php";        
    static string getBlockMenuOracle = $"{baseUrl}GetMenuOracle_Id_Name_Blocked_Icon.php";        
    static string updateBlockMenuOracle = $"{baseUrl}UpdateBlockMenuOracle.php";        
    static string getOrdersDoneOracle = $"{baseUrl}GetOrdersDoneOracle.php";        

    public static IEnumerator GetMenuOracle(Action<string> callBack)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(menuOracle))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = menuOracle.Split('/');
            int page = pages.Length - 1;            

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log($"{pages[page]}: {webRequest.downloadHandler.text}");

                    callBack?.Invoke(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public static async Task GetMenuOracleAsync(CancellationToken cancellationToken, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(menuOracle))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Prompt.Instance.ShowTooltip(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task LoginOracleAsync(string username, string password, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Nazwa", username);
        form.AddField("Pass", password);

        using (UnityWebRequest www = UnityWebRequest.Post(loginOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                callBack?.Invoke(www.downloadHandler.text);       
            }
        }
    }

    public static async Task RegisterOracleAsync(string username, string password, string phoneNumber, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Nazwa", username);
        form.AddField("Pass", password);
        form.AddField("Tel", phoneNumber);

        using (UnityWebRequest www = UnityWebRequest.Post(registerOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Prompt.Instance.ShowTooltip($"Error: {www.error}");
            }
            else if (www.downloadHandler.text.Contains("User already exists!"))
            {
                Prompt.Instance.ShowTooltip("User already exists!");
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                //SceneManager.LoadScene("LoginMenu");
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static IEnumerator AllUsersOracle(Action<string> callBack)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(allUsersOracle))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = menuOracle.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log($"{pages[page]}: {webRequest.downloadHandler.text}");

                    callBack?.Invoke(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public static IEnumerator AllWorkersOracle(Action<string> callBack)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(workersOracle))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = menuOracle.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log($"{pages[page]}: {webRequest.downloadHandler.text}");

                    callBack?.Invoke(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public static IEnumerator RegisterWorkerOracle(string username, string password, string type)
    {
        WWWForm form = new WWWForm();
        form.AddField("Nazwa", username);
        form.AddField("Pass", password);
        form.AddField("Type", type);

        UnityWebRequest www = UnityWebRequest.Post(registerWorkerOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else if (www.downloadHandler.text.Contains("User already exists!"))
        {
            Debug.Log("Try Again");
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            SceneManager.LoadScene("AdminMenu");
        }
    }

    public static IEnumerator UpdateWorkerOracle(string id, string username, string haslo, string type, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Id", id);
        form.AddField("Nazwa", username);
        form.AddField("Haslo", haslo);
        form.AddField("Typ", type);

        UnityWebRequest www = UnityWebRequest.Post(updateWorkerOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            callBack?.Invoke(www.downloadHandler.text);
        }
    }

    public static IEnumerator DeleteWorkerOracle(string id, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Id", id);

        UnityWebRequest www = UnityWebRequest.Post(deleteWorkerOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            callBack?.Invoke(www.downloadHandler.text);
        }
    }

    public static IEnumerator InsertMenuOracle(string name, string price, string blocked, string description, string icon, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Name", name);
        form.AddField("Price", price);
        form.AddField("Blocked", blocked);
        form.AddField("Description", description);
        form.AddField("Icon", icon);

        UnityWebRequest www = UnityWebRequest.Post(insertMenuOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            callBack?.Invoke(www.downloadHandler.text);
        }
    }

    public static IEnumerator UpdateMenuOracle(string id, string name, string price, string blocked, string description, string icon, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Id", id);
        form.AddField("Name", name);
        form.AddField("Price", price);
        form.AddField("Blocked", blocked);
        form.AddField("Description", description);
        form.AddField("Icon", icon);

        UnityWebRequest www = UnityWebRequest.Post(updateMenuOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            callBack?.Invoke(www.downloadHandler.text);
        }
    }

    public static IEnumerator DeleteMenuOracle(string id, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Id", id);

        UnityWebRequest www = UnityWebRequest.Post(deleteMenuOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            callBack?.Invoke(www.downloadHandler.text);
        }
    }

    public static async Task GetOrderConfigurationOracleAsync(string id, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Id", id);

        using (UnityWebRequest www = UnityWebRequest.Post(getOrderConfigurationOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task InsertZamowienieOracleAsync(string idKlienta, string status, string zamowione, string opis, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("IdKlienta", idKlienta);
        form.AddField("Status", status);
        form.AddField("Zamowione", zamowione);
        form.AddField("Opis", opis);

        using (UnityWebRequest www = UnityWebRequest.Post(insertZamowienieOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task InsertKonfiguracjeOracleAsync(string produkt, string cena, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Produkt", produkt);
        form.AddField("Cena", cena);

        using (UnityWebRequest www = UnityWebRequest.Post(insertKonfiguracjeOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task GetMenuOracleAsync_Id_Name(CancellationToken cancellationToken, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getMenuOracle_Id_Name))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static IEnumerator InsertOptionsConfigurationOracle(string idPozycji, string idOpcji, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID_Pozycji", idPozycji);
        form.AddField("ID_Opcji", idOpcji);

        UnityWebRequest www = UnityWebRequest.Post(insertOptionsConfigurationOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            callBack?.Invoke(www.downloadHandler.text);
        }
    }

    public static async Task GetOnlyConfigurationOracle(CancellationToken cancellationToken, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getOnlyConfigurationOracle))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task GetConfigurationMenuItemsOracle(string IdOpcji, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID_opcji", IdOpcji);

        using (UnityWebRequest www = UnityWebRequest.Post(getConfigurationMenuItemsOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task DeleteConfigurationOracle(string IdOpcji, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID_opcji", IdOpcji);

        using (UnityWebRequest www = UnityWebRequest.Post(deleteConfigurationOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task UpdateConfiguration(string IdOpcji, string produkt, string cena, string IdPozycjiDodaj, string IdPozycjiUsun, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID_opcji", IdOpcji);
        form.AddField("Produkt", produkt);
        form.AddField("Cena", cena);
        form.AddField("ID_pozycji", IdPozycjiDodaj);
        form.AddField("ID_pozycji_do_usuniecia", IdPozycjiUsun);

        using (UnityWebRequest www = UnityWebRequest.Post(updateConfigurationOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task GetOrderOracle(CancellationToken cancellationToken, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getOrdersOracle))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task UpdateStatucOracle(string IdZamowienia, string Status, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID_zamowienia", IdZamowienia);
        form.AddField("Status", Status);

        using (UnityWebRequest www = UnityWebRequest.Post(updateStatusOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task GetWarehouseOracle(CancellationToken cancellationToken, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getWarehouseOracle))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task DeleteWarehouseOracle(string IDProduktu, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID_produktu", IDProduktu);

        using (UnityWebRequest www = UnityWebRequest.Post(deleteWarehouseOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task InsertWarehouseOracle(string nazwa, string ilosc, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("Nazwa", nazwa);
        form.AddField("Ilosc", ilosc);

        using (UnityWebRequest www = UnityWebRequest.Post(insertWarehouseOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task UpdateWarehouseOracle(string IdProduktu, string ilosci, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID_produktow", IdProduktu);
        form.AddField("Ilosci", ilosci);

        using (UnityWebRequest www = UnityWebRequest.Post(updateWarehouseOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task GetMenu_Id_Name_Blocked_IconOracle(CancellationToken cancellationToken, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(menuOracle))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Prompt.Instance.ShowTooltip(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task UpdateBlockMenuOracle(string IdsBlock, string IdsUnblock, CancellationToken cancellationToken, Action<string> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("IdsBlock", IdsBlock);
        form.AddField("IdsUnblock", IdsUnblock);

        using (UnityWebRequest www = UnityWebRequest.Post(updateBlockMenuOracle, form))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public static async Task GetOrdersDoneOracle(CancellationToken cancellationToken, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getOrdersDoneOracle))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task was cancelled.");
                    www.Abort();
                    return;
                }
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callBack?.Invoke(www.downloadHandler.text);
            }
        }
    }
}

[Serializable]
public class UserInfo
{
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Password { get; private set; }
    public string PhoneNumber { get; private set; } = null;
    public string WorkerType { get; private set; } = null;

    public bool ClientLoggedIn { get; private set; }

    public void SetClientCridentials(string username, string userPassword, ClientDataJson data)
    {
        ID = data.ID;
        Name = username;
        Password = userPassword;
        PhoneNumber = data.Tel;

        ClientLoggedIn = true;
    }

    public void SetWorkerCridentials(string username, string userPassword, WorkerDataJson data)
    {
        ID = data.ID;
        Name = username;
        Password = userPassword;
        WorkerType = data.Type;
    }

    [Serializable]
    public struct ClientDataJson
    {
        [JsonProperty("ID_klienta")]
        public string ID;

        [JsonProperty("Nrtelefonu")]
        public string Tel;
    }

    [Serializable]
    public struct WorkerDataJson
    {
        [JsonProperty("ID_pracownika")]
        public string ID;

        [JsonProperty("Typ_pracownika")]
        public string Type;
    }
}



