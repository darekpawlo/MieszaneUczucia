using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OracleGetMenu : MonoBehaviour
{
    private List<TestData> data;
    private void Start()
    {
        StartCoroutine(WebActions.GetMenuOracle((menuItemJson) =>
        {
            data = JsonConvert.DeserializeObject<List<TestData>>(menuItemJson);

            Sprite icon = ImageManager.BytesToSprite(Convert.FromBase64String(data[0].Icon));
            
        }));
    }
}

[Serializable]
public struct TestData
{
    [JsonProperty("ID_pozycji")]
    public string Id;

    [JsonProperty("Nazwa produktu")]
    public string Name;

    [JsonProperty("zablokowane")]
    public string Blocked;

    [JsonProperty("skladniki")]
    public string Description;

    [JsonProperty("zdjecia")]
    public string Icon;
}
