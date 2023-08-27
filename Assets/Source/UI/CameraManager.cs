using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public CinemachineBrain cinemachineBrain;

    public Character focusedCharacter;
    public Queue<Character> cameraFocusQueue = new Queue<Character>();

    public enum CameraFollowMode
    {
        Auto,
        Manual,
        Player
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
        Character player = WebSocketClient.Instance.player;
        if (player != null)
        {
            this.focusedCharacter = WebSocketClient.Instance.player;

            currentCameraFollowMode = CameraFollowMode.Player;
            cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

            player.camera.Priority = 100;

            UpdateUI(player);
        }
    }

    bool autoSwitchStarted = false;

    void Update()
    {
        if (currentCameraFollowMode == CameraFollowMode.Player && this.focusedCharacter != WebSocketClient.Instance.player)
        {
           // Do nothing
        }
        else if (currentCameraFollowMode == CameraFollowMode.Auto)
        {
            if (currentCameraFollowMode == CameraFollowMode.Auto && !autoSwitchStarted)
            {
                StartCoroutine(AutoSwitchCameraFocus());
                autoSwitchStarted = true;
            }
            else if (currentCameraFollowMode != CameraFollowMode.Auto)
            {
                autoSwitchStarted = false;
            }

            //if (focusedCharacter != cameraFocusQueue.Peek())
            //{
            //    SwitchCameraFocus(cameraFocusQueue.Peek());
            //}
        }
        else if (currentCameraFollowMode == CameraFollowMode.Manual)
        {
            // Only switch between active characters
            List<Character> characters = WebSocketClient.Instance.characters
                                                       .Where(character => character.gameObject.activeInHierarchy)
                                                       .ToList();

            // If user presses key from 1-9, select that character in the list specifically
            for (int i = 0; i < Mathf.Min(9, characters.Count); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SwitchCameraFocus(characters[i]);
                    return;
                }
            }

            // Otherwise check for + or - 
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

    IEnumerator AutoSwitchCameraFocus()
    {
        List<Character> characters = new List<Character>();

        while (currentCameraFollowMode == CameraFollowMode.Auto)
        {
            // Refresh the list of active characters
            characters = WebSocketClient.Instance.characters
                                          .Where(character => character.gameObject.activeInHierarchy)
                                          .ToList();

            int characterIndex = characters.IndexOf(focusedCharacter);
            characterIndex = (characterIndex + 1) % characters.Count;

            SwitchCameraFocus(characters[characterIndex]);

            yield return new WaitForSeconds(10); // Wait for 3 seconds
        }
    }


    public void SwitchCameraFocus(Character character)
    {

        if (this.focusedCharacter == character)
        {
            return;
        }

        this.focusedCharacter = character;
        
        character.camera.Priority = (int)((Time.time * 100) + 1); // The 1 is for when we call it on Start() and time.time is 0

        UpdateUI(character);
    }

    public void UpdateUI(Character character)
    {
        // Inventory Manager
        if (character is IHasInventory iHasInventory && character.IsPlayerControlled)
        {
            // TODO: make this a function on inventory UI manager
            InventoryUIManager.Instance.displayedInventory = iHasInventory.EntityInventory;
            iHasInventory.EntityInventory.subscribedUI = InventoryUIManager.Instance;

            InventoryUIManager.Instance.Render();

            InventoryUIManager.Instance.gameObject.SetActive(true);
        }
        else
        {
            InventoryUIManager.Instance.gameObject.SetActive(false);
        }

        // Satiety Bar / Stats Bar
        if (character is IHasStomach iHasStomach && character.IsPlayerControlled)
        {
            StatUIManager.Instance.displayedSatiety = iHasStomach;
            StatUIManager.Instance.gameObject.SetActive(true);
        } else
        {
            StatUIManager.Instance.gameObject.SetActive(false);
        }

        // Dialogue Manager
        if (DialogueUIManager.Instance.activeConversation != null && DialogueUIManager.Instance.activeConversation.character != character && DialogueUIManager.Instance.activeConversation.targetCharacter != character)
        {
            DialogueUIManager.Instance.SetActiveConvseration(null);

            if (character.CurrentAction is StartConversation)
            {
                DialogueUIManager.Instance.SetActiveConvseration((StartConversation)character.CurrentAction);
            }
        }
        else if (DialogueUIManager.Instance.activeConversation == null)
        {
            if (character.CurrentAction is StartConversation)
            {
                DialogueUIManager.Instance.SetActiveConvseration((StartConversation)character.CurrentAction);
            }
        }
    }
}
