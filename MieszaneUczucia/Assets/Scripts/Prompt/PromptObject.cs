using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptObject : MonoBehaviour
{
    public Prompt prompt;
    public void SpawnPrompt()
    {
        Instantiate(gameObject);
    }
}
