using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Items : MonoBehaviour
{
    Action<string> createItemsCallback;
    Func<string> getItemsCallback;

    private void Start()
    {
        createItemsCallback = (jsonArrayString) => 
        {
            StartCoroutine(CreateItemRoutine(jsonArrayString));
        };

        CreateItems();
    }

    public void CreateItems()
    {
        StartCoroutine(WebActions.GetItemsIDs(WebActions.UserInfo.ID, createItemsCallback));
    }

    IEnumerator CreateItemRoutine(string jsonArrayString)
    {
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;

        for (int i = 0; i < jsonArray.Count; i++)
        {
            //Create local variables
            bool isDone = false;
            string itemId = jsonArray[i].AsObject["itemID"];
            JSONObject itemInfoJson = new JSONObject();

            //Create a callback to get the information from WebActions
            Action<string> getItemInfoCallback = (itemInfo) =>
            {
                isDone = true;
                JSONArray tempArray = JSON.Parse(itemInfo) as JSONArray;
                itemInfoJson = tempArray[0].AsObject;
            };

            StartCoroutine(WebActions.GetItem(itemId, getItemInfoCallback));

            yield return new WaitUntil(() => isDone == true);

            GameObject item = Instantiate(Resources.Load("Prefabs/Item") as GameObject);
            item.transform.SetParent(transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            item.transform.Find("Name").GetComponent<TMP_Text>().text = itemInfoJson["name"];
            item.transform.Find("Price").GetComponent<TMP_Text>().text = itemInfoJson["price"];
            item.transform.Find("Description").GetComponent<TMP_Text>().text = itemInfoJson["description"];
        }

        yield return null;
    }
}