using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class Items : MonoBehaviour
{
    Action<string> createItemsCallback;

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
        var itemIDs = JsonConvert.DeserializeObject<List<ItemIDs>>(jsonArrayString);

        for (int i = 0; i < itemIDs.Count; i++)
        {
            //Create local variables
            bool isDone = false;
            string itemId = itemIDs[i].itemID;

            ItemData itemInfoJson = new ItemData();
            //Create a callback to get the information from WebActions
            Action<string> getItemInfoCallback = (itemInfo) =>
            {
                isDone = true;
                var tempArray = JsonConvert.DeserializeObject<List<ItemData>>(itemInfo);
                itemInfoJson = tempArray[0];
            };
            StartCoroutine(WebActions.GetItem(itemId, getItemInfoCallback));
            yield return new WaitUntil(() => isDone == true);

            var item = Instantiate(Resources.Load("Prefabs/Item") as GameObject, transform.position, Quaternion.identity, transform);
            item.transform.Find("Name").GetComponent<TMP_Text>().text = itemInfoJson.name;
            item.transform.Find("Price").GetComponent<TMP_Text>().text = itemInfoJson.price;
            item.transform.Find("Description").GetComponent<TMP_Text>().text = itemInfoJson.description;
        }

        yield return null;
    }
}

[Serializable]
public class ItemIDs
{
    public string itemID;
}

[Serializable]
public class ItemData
{
    public string name;
    public string description;
    public string price;
}