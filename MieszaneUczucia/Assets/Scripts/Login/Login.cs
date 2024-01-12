using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;

public class Login : MonoBehaviour
{
    [SerializeField] TMP_InputField userInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] Button loginButton;
    [SerializeField] Button backButton;
    [SerializeField] Transform loginPanel;
    [SerializeField] bool userLoginOnly;

    TaskManager taskManager = new TaskManager();

    private void Start()
    {
        loginButton.onClick.AddListener(() =>
        {
            CallWebAction();
        });

        backButton.onClick.AddListener(() =>
        {
            taskManager.CancelCurrentTask();
        });
    }

    public void GoToRegister() => SceneManager.LoadScene("RegisterMenu");
    public void GoToMenu() => SceneManager.LoadScene("Menu");

    async void CallWebAction()
    {
        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();
            await WebActions.LoginOracleAsync(userInput.text, passInput.text, cancellationToken, (text) =>
            {
                tcs.SetResult(text);
            });
            string responseText = await tcs.Task;
            cancellationToken.ThrowIfCancellationRequested();

            if (responseText.Contains("Wrong credentials") || responseText.Contains("Username does not exist!"))
            {
                Prompt.Instance.ShowTooltip(responseText);
                passInput.text = "";
            }
            else
            {
                if(userLoginOnly)
                {
                    // Klient
                    if (responseText.Contains("Nrtelefonu"))
                    {
                        var jsonData = JsonConvert.DeserializeObject<List<UserInfo.ClientDataJson>>(responseText);
                        WebActions.UserInfo.SetClientCridentials(userInput.text, passInput.text, jsonData[0]);

                        Prompt.Instance.ShowTooltip("Zalogowanie powiod³o siê!", () =>
                        {
                            loginPanel.gameObject.SetActive(false);
                            BasketManager.Instance.UpdateBottomBar();
                        });
                    }
                    // Pracownicy
                    else if (responseText.Contains("Typ_pracownika"))
                    {
                        Prompt.Instance.ShowTooltip("Try Again!");
                        passInput.text = "";
                    }
                }     
                else
                {
                    ///////////////////////////Klient//////////////////////////////////////////////
                    if (responseText.Contains("Nrtelefonu"))
                    {
                        Prompt.Instance.ShowTooltip("Try Again!");
                        userInput.text = "";
                        passInput.text = "";
                    }
                    //////////////////////////Pracownicy///////////////////////////////////////////
                    else if (responseText.Contains("Typ_pracownika"))
                    {
                        var jsonData = JsonConvert.DeserializeObject<List<UserInfo.WorkerDataJson>>(responseText);
                        WebActions.UserInfo.SetWorkerCridentials(userInput.text, passInput.text, jsonData[0]);

                        if (responseText.Contains("Menedzer"))
                        {
                            Debug.Log("Menedzer");
                            SceneManager.LoadScene("ManagerMenu");
                        }
                        else if (responseText.Contains("Admin"))
                        {
                            Debug.Log("Admin");
                            SceneManager.LoadScene("AdminMenu");
                        }
                    }
                }
            }
        });
    }
}
