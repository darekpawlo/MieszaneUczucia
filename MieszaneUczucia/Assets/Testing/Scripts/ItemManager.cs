using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    [Serializable]
    struct ItemIDs
    {
        public string id;
        public string itemID;
    }

    [Serializable]
    struct ItemData
    {
        public string name;
        public string description;
        public string price;
    }

    [SerializeField] private Item itemPrefab;

    private void Start()
    {
        StartCoroutine(WebActions.GetItemsIDs(WebActions.UserInfo.ID, (jsonArray) =>
        {
            var itemIDs = JsonConvert.DeserializeObject<List<ItemIDs>>(jsonArray);
            CreateItemRoutine(itemIDs);
        }));
    }

    void CreateItemRoutine(List<ItemIDs> itemIDs)
    {       
        void SetImage(Item item, string itemId)
        {
            ImageManager.CheckDirectory();
            byte[] bytes = ImageManager.LoadImage(itemId);
            //Download from web
            if (bytes.Length == 0)
            {
                StartCoroutine(WebActions.GetItemIcon(itemId, (downloadBytes) =>
                {
                    item.transform.Find("Image").GetComponent<Image>().sprite = ImageManager.BytesToSprite(downloadBytes);
                    ImageManager.SaveImage(itemId, downloadBytes);
                }));
            }
            //Load from device
            else
            {
                item.transform.Find("Image").GetComponent<Image>().sprite = ImageManager.BytesToSprite(bytes);
            }
        }
        for (int i = 0; i < itemIDs.Count; i++)
        {
            string itemId = itemIDs[i].itemID;
            string id = itemIDs[i].id;

            StartCoroutine(WebActions.GetItem(itemId, (itemInfo) =>
            {
                var tempArray = JsonConvert.DeserializeObject<List<ItemData>>(itemInfo);
                var itemInfoJson = tempArray[0];

                var item = Instantiate(itemPrefab, transform.position, Quaternion.identity, transform);

                item.ID = id;
                item.ItemID = itemId;

                item.transform.Find("Name").GetComponent<TMP_Text>().text = itemInfoJson.name;
                item.transform.Find("Price").GetComponent<TMP_Text>().text = itemInfoJson.price;
                item.transform.Find("Description").GetComponent<TMP_Text>().text = itemInfoJson.description;
                item.transform.Find("SellButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    StartCoroutine(WebActions.SellItem(id, itemId, WebActions.UserInfo.ID, () =>
                    {
                        Destroy(item.gameObject);
                    }));
                });

                SetImage(item, itemId);
            }));
        }
    }
}

