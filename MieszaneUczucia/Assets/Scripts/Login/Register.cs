using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    [SerializeField] TMP_InputField userInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] TMP_InputField phoneInput;
    [SerializeField] TMP_InputField confirmInput;
    [SerializeField] Button registerButton;

    [SerializeField] Transform registerPanel;
    [SerializeField] Transform loginPanel;
    [SerializeField] bool usingPanels;

    TaskManager taskManager = new TaskManager();

    private void Start()
    {
        registerButton.onClick.AddListener(async () =>
        {
            if(passInput.text != confirmInput.text)
            {
                Prompt.Instance.ShowTooltip("Passwords don't match");
                return;
            }

            if (phoneInput.text.Length > 9 || phoneInput.text.Length < 9)
            {
                Prompt.Instance.ShowTooltip("Not correct phone number");
                return;
            }

            if (userInput.text == string.Empty || passInput.text == string.Empty || confirmInput.text == string.Empty || phoneInput.text == string.Empty)
            {
                Prompt.Instance.ShowTooltip("Not all fields are filled");
                return;
            }

            if(!taskManager.IsTaskRunning)
            {
                Prompt.Instance.ShowLoadingBar();
                await CallWebAction();
            }
            else
            {
                Debug.Log("Registration already in progress");
            }
        });
    }

    public void GoToLogin()
    {
        taskManager.CancelCurrentTask();
        SceneManager.LoadScene("Menu");
    }

    async Task CallWebAction()
    {
        taskManager.CancelCurrentTask();

        await taskManager.RunTaskAsync(async cancellationToken =>
        {
            var tcs = new TaskCompletionSource<string>();
            await WebActions.RegisterOracleAsync(userInput.text, passInput.text, phoneInput.text, cancellationToken, (text) =>
            {
                tcs.SetResult(text);
            });
            string responseText = await tcs.Task;
            cancellationToken.ThrowIfCancellationRequested();

            if(responseText.Contains("Created user"))
            {
                if(usingPanels)
                {
                    Prompt.Instance.ShowTooltip("Rejestracja powiod³a siê", () =>
                    {
                        loginPanel.gameObject.SetActive(true);
                        registerPanel.gameObject.SetActive(false);
                    });
                }
                else
                {
                    Prompt.Instance.ShowTooltip("Rejestracja powiod³a siê");
                }

                //string prefix = "Created user: ";
                //int indexOfNumber = responseText.IndexOf(prefix) + prefix.Length;
                //string numberString = responseText.Substring(indexOfNumber).Trim(); // Usuniêcie spacji na pocz¹tku i na koñcu, jeœli takie istniej¹

                //PlayerPrefs.SetString("ID_klienta", numberString);
            }
        });
    }
}
