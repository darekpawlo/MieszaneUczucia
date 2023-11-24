using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [SerializeField] TMP_InputField userInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] Button loginButton;
    [SerializeField] TMP_Text prompt;
    [SerializeField] TMP_Text prompt2;

    private void Start()
    {
        loginButton.onClick.AddListener(() =>
        {
            StartCoroutine(WebActions.Login(userInput.text, passInput.text, prompt, prompt2));
        });
    }

    public void GoToRegister() => SceneManager.LoadScene("RegisterMenu");
    public void GoToMenu() => SceneManager.LoadScene("Menu");
}
