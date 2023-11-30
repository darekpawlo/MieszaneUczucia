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

    static string swiatolowod = "192.168.1.24";
    static string router = "192.168.8.108";

    public static string uzywaneIP = swiatolowod;
    static string baseUrl = $"http://{uzywaneIP}/UnityBackendTutorial/";

    static string loginUrl = $"{baseUrl}Login.php";
    static string registerUrl = $"{baseUrl}RegisterUser.php";
    static string itemsIDs = $"{baseUrl}GetItemsIDs.php";
    static string item = $"{baseUrl}GetItem.php";
    static string sellItem = $"{baseUrl}SellItem.php";
    static string itemIcon = $"{baseUrl}GetItemIcon.php";
    static string menu = $"{baseUrl}GetMenu.php";
    static string menuOracle = $"{baseUrl}GetMenuOracle.php";
    static string loginOracle = $"{baseUrl}LoginOracle.php";
    static string registerOracle = $"{baseUrl}RegisterOracle.php";
    static string allUsersOracle = $"{baseUrl}GetAllAccounts.php";
    static string registerWorkerOracle = $"{baseUrl}RegisterWorkerOracle.php";

    public static IEnumerator GetUsers(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
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
                    break;
            }           
        }
    }

    public static IEnumerator Register(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("login", username);
        form.AddField("pass", password);

        UnityWebRequest www = UnityWebRequest.Post(registerUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            SceneManager.LoadScene("LoginMenu");
        }
    }

    public static IEnumerator GetItemsIDs(string userID, Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(itemsIDs, form))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = itemsIDs.Split('/');
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
                    string jsonArray = webRequest.downloadHandler.text;

                    callback?.Invoke(jsonArray);
                    break;
            }
        }
    }

    public static IEnumerator GetItem(string itemID, Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("itemID", itemID);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(item, form))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = item.Split('/');
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
                    string jsonArray = webRequest.downloadHandler.text;

                    callback?.Invoke(jsonArray);
                    break;
            }
        }
    }

    public static IEnumerator SellItem(string id, string itemID, string userID, Action callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("itemID", itemID);
        form.AddField("userID", userID);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(sellItem, form))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = item.Split('/');
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

                    callback?.Invoke();
                    break;
            }
        }
    }

    public static IEnumerator GetItemIcon(string itemID, Action<byte[]> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("itemID", itemID);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(itemIcon, form))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = itemIcon.Split('/');
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

                    callback?.Invoke(webRequest.downloadHandler.data);
                    break;
            }
        }
    }

    public static IEnumerator GetMenu(Action<string> callBack)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(menu))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = menu.Split('/');
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



