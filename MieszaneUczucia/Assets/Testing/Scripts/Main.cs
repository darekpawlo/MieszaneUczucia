using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main instance;

    /*public WebActions WebActions { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        WebActions = GetComponent<WebActions>();
    }*/
}
