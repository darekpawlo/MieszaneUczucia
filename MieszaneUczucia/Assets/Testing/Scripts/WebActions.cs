using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public static class WebActions
{
    public static UserInfo UserInfo = new UserInfo();

    static string swiatolowod = "192.168.1.24";
    static string router = "192.168.8.108";

    static string uzywaneIP = swiatolowod;
    static string baseUrl = $"http://{uzywaneIP}/UnityBackendTutorial/";

    static string loginUrl = $"{baseUrl}Login.php";
    static string registerUrl = $"{baseUrl}RegisterUser.php";
    static string itemsIDs = $"{baseUrl}GetItemsIDs.php";
    static string item = $"{baseUrl}GetItem.php";
    static string sellItem = $"{baseUrl}SellItem.php";
    static string itemIcon = $"{baseUrl}GetItemIcon.php";

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

    public static IEnumerator Login(string username, string password, TMP_Text prompt, TMP_Text prompt2)
    {    
        WWWForm form = new WWWForm();
        form.AddField("login", username);
        form.AddField("pass", password);

        UnityWebRequest www = UnityWebRequest.Post(loginUrl, form);
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
            if(www.downloadHandler.text.Contains("Wrong cridentials") || www.downloadHandler.text.Contains("Username does not exist!"))
            {
                Debug.Log("Try Again");
            }
            else
            {
                //If we logged in correctly
                UserInfo.SetCredentials(username, password);
                UserInfo.SetID(www.downloadHandler.text);

                SceneManager.LoadScene(4);
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

    public static IEnumerator GetItemIcon(string itemID, Action<Sprite> callback)
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

                    byte[] bytes = webRequest.downloadHandler.data;

                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(bytes);

                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    callback?.Invoke(sprite);
                    break;
            }
        }
    }
}

[Serializable]
public class UserInfo
{
    public string ID { get; private set; }  
    string Name;
    string Password;
    string Level;
    string Coins;

    public void SetCredentials(string username, string userPassword)
    {
        Name = username; 
        Password = userPassword;
    }

    public void SetID(string id)
    {
        ID = id;
    }
}
