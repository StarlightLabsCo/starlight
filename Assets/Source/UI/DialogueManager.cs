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

    int speakingWPM = 400;

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

    public void DisplayDialogue(string name, string dialogueText, System.Action callback)
    {
        float secondsToSpeak = (dialogueText.Length / 4.7f) / (speakingWPM / 60f);

        this.name.text = name;
        StartCoroutine(RevealDialogue(dialogueText, secondsToSpeak, callback));
    }

    private IEnumerator RevealDialogue(string dialogueText, float seconds, System.Action callback)
    {
        dialogueBox.SetActive(true);

        float charSeconds = seconds / dialogueText.Length;

        string[] words = dialogueText.Split(' ');
        int i = 0;

        while (i < words.Length)
        {
            string part = "";
            while (i < words.Length && (part + words[i]).Length <= 185)
            {
                part += words[i] + " ";
                i++;
            }

            yield return StartCoroutine(RevealPart(part, charSeconds * part.Length));
        }

        yield return new WaitForSeconds(3);
        Clear();
        callback(); 
    }

    private IEnumerator RevealPart(string part, float partSeconds)
    {
        dialogue.text = "";
        foreach (char c in part)
        {
            dialogue.text += c;
            yield return new WaitForSeconds(partSeconds / part.Length);
        }
    }

    public void Clear()
    {
        dialogueBox.SetActive(false);
    }
}
