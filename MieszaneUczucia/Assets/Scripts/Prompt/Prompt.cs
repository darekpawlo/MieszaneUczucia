using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Prompt : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text promptText;
    [SerializeField] RectTransform promptPanel;

    public void ShowTooltip(string message)
    {
        gameObject.SetActive(true);

        promptText.text = message;

        Vector2 backgroundSize = new Vector2(promptText.preferredWidth, promptText.preferredHeight);
        promptPanel.sizeDelta = backgroundSize;
    }

    public void HideTooltip() 
    {
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HideTooltip();
    }
}
