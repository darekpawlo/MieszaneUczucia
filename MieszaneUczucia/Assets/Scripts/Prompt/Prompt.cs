using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Prompt : MonoBehaviour, IPointerClickHandler
{
    public static Prompt Instance { get; private set; }

    [SerializeField] TMP_Text promptText;
    [SerializeField] RectTransform promptPanel;

    [Header("LoadingBar")]
    [SerializeField] GameObject loadingBarPanel;
    [SerializeField] Image loadingBar;
    float barValue = 0;

    Action action;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(transform.root.gameObject);

        promptPanel.gameObject.SetActive(false);
        loadingBarPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(loadingBarPanel.gameObject.activeInHierarchy)
        {
            barValue += 0.01f;
            loadingBar.fillAmount = barValue;
            if (barValue >= 1) barValue = 0;
        }
    }

    public void ShowTooltip(string message, Action action = null)
    {
        this.action = action;

        promptPanel.gameObject.SetActive(true);
        loadingBarPanel.gameObject.SetActive(false);

        promptText.text = message;
    }

    public void ShowLoadingBar(Action action = null)
    {
        this.action = action;

        loadingBarPanel.gameObject.SetActive(true);
    }

    public void HideTooltip() 
    {
        promptPanel.gameObject.SetActive(false);
    }

    public void HideLoadingBar()
    {
        loadingBarPanel.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        action?.Invoke();

        HideTooltip();
        //HideLoadingBar();
    }
}
