using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class Items : MonoBehaviour
{
    [Serializable]
    struct ItemIDs
    {
        public string itemID;
    }

    [Serializable]
    struct ItemData
    {
        public string name;
        public string description;
        public string price;
    }

    private void Start()
    {
        StartCoroutine(WebActions.GetItemsIDs(WebActions.UserInfo.ID, (jsonArray) =>
        {
            CreateItemRoutine(jsonArray);
        }));
    }

    void CreateItemRoutine(string jsonArrayString)
    {
        var itemIDs = JsonConvert.DeserializeObject<List<ItemIDs>>(jsonArrayString);

        for (int i = 0; i < itemIDs.Count; i++)
        {
            string itemId = itemIDs[i].itemID;
            StartCoroutine(WebActions.GetItem(itemId, (itemInfo) =>
            {
                var tempArray = JsonConvert.DeserializeObject<List<ItemData>>(itemInfo);
                var itemInfoJson = tempArray[0];

                var item = Instantiate(Resources.Load("Prefabs/Item") as GameObject, transform.position, Quaternion.identity, transform);
                item.transform.Find("Name").GetComponent<TMP_Text>().text = itemInfoJson.name;
                item.transform.Find("Price").GetComponent<TMP_Text>().text = itemInfoJson.price;
                item.transform.Find("Description").GetComponent<TMP_Text>().text = itemInfoJson.description;
            }));
        }
    }
}

