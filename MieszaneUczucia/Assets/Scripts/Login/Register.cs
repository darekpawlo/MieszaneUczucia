using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;
using System.Linq;

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

    public void GoToLogin()
    {
        taskManager.CancelCurrentTask();
        SceneManager.LoadScene("Menu");
    }

    public void RegisterAccount()
    {
        if (passInput.text != confirmInput.text)
        {
            Prompt.Instance.ShowTooltip("Has�a nie sa takie same");
            return;
        }

        if (phoneInput.text.Length > 9 || phoneInput.text.Length < 9)
        {
            Prompt.Instance.ShowTooltip("Niepoprawny numer telefonu");
            return;
        }

        if(passInput.text.Length < 8)
        {
            Prompt.Instance.ShowTooltip("Has�o musi mie� przynajmniej 8 znak�w");
            return;
        }

        bool IsLetter(char c) => c >= 'A' && c <= 'Z';
        bool IsDigit(char c) => c >= '0' && c <= '9';
        bool IsSymbol(char c) => c > 32 && c < 127 && !IsDigit(c) && !IsLetter(c);
        bool IsValidPassword(string password)
        {
            return
               password.Any(c => IsLetter(c)) &&
               password.Any(c => IsDigit(c)) &&
               password.Any(c => IsSymbol(c));
        }

        if (!IsValidPassword(passInput.text))
        {
            Prompt.Instance.ShowTooltip("Has�o musi zawiera� przynajmniej jeden znak specjalny, cyfr� i du�� litere");
            return;
        }

        if (userInput.text == string.Empty || passInput.text == string.Empty || confirmInput.text == string.Empty || phoneInput.text == string.Empty)
        {
            Prompt.Instance.ShowTooltip("Wype�nij wszystkie pola");
            return;
        }

        if (!taskManager.IsTaskRunning)
        {
            Prompt.Instance.ShowLoadingBar();
            CallWebAction();
        }
        else
        {
            Debug.Log("Registration already in progress");
        }
    }

    async void CallWebAction()
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
                    Prompt.Instance.ShowTooltip("Rejestracja powiod�a si�", () =>
                    {
                        loginPanel.gameObject.SetActive(true);
                        registerPanel.gameObject.SetActive(false);
                    });
                }
                else
                {
                    Prompt.Instance.ShowTooltip("Rejestracja powiod�a si�");
                }

                //string prefix = "Created user: ";
                //int indexOfNumber = responseText.IndexOf(prefix) + prefix.Length;
                //string numberString = responseText.Substring(indexOfNumber).Trim(); // Usuni�cie spacji na pocz�tku i na ko�cu, je�li takie istniej�

                //PlayerPrefs.SetString("ID_klienta", numberString);
            }
        });
    }
}
