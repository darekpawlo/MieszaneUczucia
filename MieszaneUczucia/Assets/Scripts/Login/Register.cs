using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    [SerializeField] TMP_InputField userInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] TMP_InputField phoneInput;
    [SerializeField] TMP_InputField confirmInput;
    [SerializeField] Button loginButton;

    private void Start()
    {
        loginButton.onClick.AddListener(() =>
        {
            if(passInput.text != confirmInput.text)
            {
                Debug.Log("Passwords don't match");
                return;
            }

            if (phoneInput.text.Length > 9 || phoneInput.text.Length < 9)
            {
                Debug.Log("Wrong phone number");
                return;
            }

            if (userInput.text == string.Empty || passInput.text == string.Empty || confirmInput.text == string.Empty || phoneInput.text == string.Empty)
            {
                Debug.Log("Fill all fields");
                return;
            }

            StartCoroutine(WebActions.RegisterOracle(userInput.text, passInput.text, phoneInput.text));      
        });
    }
}
