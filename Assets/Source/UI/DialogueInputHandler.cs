using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueInputHandler : MonoBehaviour
{

    public static DialogueInputHandler Instance { get; private set; }

    public TMP_InputField inputField;

    public delegate void EndEditCallback(string text);
    private EndEditCallback endEditCallback;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        this.gameObject.SetActive(false);
    }

    public void GetInputFromPlayer(EndEditCallback _endEditCallback)
    {
        this.gameObject.SetActive(true);
        endEditCallback = _endEditCallback;
    }

    public void EndEdit(string text)
    {
        if (endEditCallback != null)
        {
            endEditCallback(text);
        }

        this.gameObject.SetActive(false);
    }

    public void Clear()
    {
        inputField.text = "";
    }
}
