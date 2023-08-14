using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField]
    public GameObject dialogueBox;

    [SerializeField]
    public TextMeshProUGUI name;

    [SerializeField]
    public TextMeshProUGUI dialogue;

    public StartConversation activeConversation;

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
    }

    void Start()
    {
        dialogueBox.SetActive(false);
    }

    public void DisplayDialogueBox()
    {
        dialogueBox.SetActive(true);
    }

    public void SetActiveConvseration(StartConversation conversation)
    {
        this.activeConversation = conversation;
    }

    public void SetDialogueDisplay(string name, string text)
    {
        this.name.text = name;
        this.dialogue.text = text;
    }

    public void Clear()
    {
        dialogueBox.SetActive(false);
        activeConversation = null;
    }
}
