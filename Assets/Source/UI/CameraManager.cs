using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Character focusedCharacter;
    public Queue<Character> cameraFocusQueue = new Queue<Character>();

    public enum CameraFollowMode
    {
        Auto,
        Manual
    }

    public CameraFollowMode currentCameraFollowMode = CameraFollowMode.Auto;

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
        List<Character> characters = WebSocketClient.Instance.characters;

        focusedCharacter = characters[0];
    }

    void Update()
    {
        if (currentCameraFollowMode == CameraFollowMode.Auto)
        {
            if (focusedCharacter != cameraFocusQueue.Peek())
            {
                SwitchCameraFocus(cameraFocusQueue.Peek());
            }
        }
        else if (currentCameraFollowMode == CameraFollowMode.Manual)
        {
            List<Character> characters = WebSocketClient.Instance.characters;

            for (int i = 0; i < Mathf.Min(9, characters.Count); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SwitchCameraFocus(characters[i]);
                    return;
                }
            }

            int characterIndex = characters.IndexOf(focusedCharacter);

            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
            {
                characterIndex = (characterIndex + 1) % characters.Count;
                SwitchCameraFocus(characters[characterIndex]);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                characterIndex = (characterIndex - 1 + characters.Count) % characters.Count;
                SwitchCameraFocus(characters[characterIndex]);
                return;
            }
        }
    }


    public void SwitchCameraFocus(Character character)
    {
        if (this.focusedCharacter == character)
        {
            return;
        }

        this.focusedCharacter = character;
        character.camera.Priority = (int)(Time.time * 100);

        if (DialogueManager.Instance.activeConversation != null && DialogueManager.Instance.activeConversation.character != character && DialogueManager.Instance.activeConversation.targetCharacter != character)
        {
            DialogueManager.Instance.SetActiveConvseration(null);

            if (character.CurrentAction is StartConversation)
            {
                DialogueManager.Instance.SetActiveConvseration((StartConversation)character.CurrentAction);
            }
        }
        else if (DialogueManager.Instance.activeConversation == null)
        {
            if (character.CurrentAction is StartConversation)
            {
                DialogueManager.Instance.SetActiveConvseration((StartConversation)character.CurrentAction);
            }
        }
    }
}
