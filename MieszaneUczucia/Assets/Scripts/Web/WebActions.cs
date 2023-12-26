using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using UnityEditor;

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

    public static IEnumerator LoginOracle(string username, string password, TMP_Text prompt, TMP_Text prompt2)
    {
        WWWForm form = new WWWForm();
        form.AddField("Nazwa", username);
        form.AddField("Pass", password);

        UnityWebRequest www = UnityWebRequest.Post(loginOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            prompt2.SetText(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            prompt.SetText(www.downloadHandler.text);
            if (www.downloadHandler.text.Contains("Wrong cridentials") || www.downloadHandler.text.Contains("Username does not exist!"))
            {
                Debug.Log("Try Again");
            }
            else
            {                
                if (www.downloadHandler.text.Contains("Nrtelefonu"))
                {
                    var jsonData = JsonConvert.DeserializeObject<List<UserInfo.ClientDataJson>>(www.downloadHandler.text);
                    UserInfo.SetClientCridentials(username, password, jsonData[0]);
                    SceneManager.LoadScene("Basket");
                }
                else if(www.downloadHandler.text.Contains("Typ_pracownika"))
                {
                    var jsonData = JsonConvert.DeserializeObject<List<UserInfo.WorkerDataJson>>(www.downloadHandler.text);
                    UserInfo.SetWorkerCridentials(username, password, jsonData[0]);

                    if(www.downloadHandler.text.Contains("Menedzer"))
                    {
                        Debug.Log("Menedzer");
                        SceneManager.LoadScene("ManagerMenu");
                    }
                    else if(www.downloadHandler.text.Contains("Admin"))
                    {
                        Debug.Log("Admin");
                        SceneManager.LoadScene("AdminMenu");
                    }
                }
            }
        }
    }

    public static IEnumerator RegisterOracle(string username, string password, string phoneNumber)
    {
        WWWForm form = new WWWForm();
        form.AddField("Nazwa", username);
        form.AddField("Pass", password);
        form.AddField("Tel", phoneNumber);

        UnityWebRequest www = UnityWebRequest.Post(registerOracle, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else if(www.downloadHandler.text.Contains("User already exists!"))
        {
            Debug.Log("Try Again");
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            SceneManager.LoadScene("LoginMenu");
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
}

[Serializable]
public class UserInfo
{
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Password { get; private set; }
    public string PhoneNumber { get; private set; } = null;
    public string WorkerType { get; private set; } = null;

    public void SetClientCridentials(string username, string userPassword, ClientDataJson data)
    {
        ID = data.ID;
        Name = username;
        Password = userPassword;
        PhoneNumber = data.Tel;
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



