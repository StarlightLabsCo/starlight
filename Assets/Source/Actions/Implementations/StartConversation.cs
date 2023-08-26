using System.Collections;
using System.Collections.Generic;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketEvents;

public class StartConversation : Action
{

    public Character character;
    public Character targetCharacter;

    string conversationGoal;

    public Queue<ConversationEvent> conversationEvents = new Queue<ConversationEvent>();
    public bool conversationFinished = false;

    const int speakingWPM = 400;
    const float secondsPerCharacter = 1 / (speakingWPM * 4.7f / 60);

    public Character currentSpeaker = null;
    public string currentlyDisplayedText = null;

    bool shouldPlayerSpeak = false;

    public StartConversation(Character character, Character targetCharacter, string conversationGoal) : base("start_conversation", "start_conversation", $"Start a conversation with {targetCharacter.Name} (ID: {targetCharacter.Id}).", JsonConvert.SerializeObject(new
    {
        type = "object",
        properties = new
        {
            characterId = new
            {
                type = "string",
                description = "The character ID of the character that is initiating the conversation."
            },
            targetCharacterId = new
            {
                type = "string",
                description = "The character ID of the target character."
            },
            conversationGoal = new
            {
                type = "string",
                description = $"Describe a hypothesis for precisely what you hope to get out of this conversation with {targetCharacter.name} in detail."
            }
        }
    })) {
        this.character = character;
        this.targetCharacter = targetCharacter;
        this.conversationGoal = conversationGoal;
    }

    public override void Execute(Character character)
    {
        Debug.Log($"Starting conversation for {character.Name}.");

        if (CameraManager.Instance.focusedCharacter == character)
        {
            DialogueUIManager.Instance.SetActiveConvseration(this);
        }

        if (this.character == character)
       {
            if (this.character.IsPlayerControlled)
            {
                if (targetCharacter.CurrentAction != null)
                {
                    targetCharacter.FinishAction();
                }
                targetCharacter.CurrentAction = this;

                InventoryUIManager.Instance.gameObject.SetActive(false);
                StatUIManager.Instance.gameObject.SetActive(false);

                DialogueUIManager.Instance.DisplayDialogueBox();
                DialogueUIManager.Instance.SetDialogueDisplay(this.character.Name, "");

                DialogueInputHandler.Instance.GetInputFromPlayer((string text) => {
                    DialogueUIManager.Instance.SetDialogueDisplay(this.character.Name, text);

                    if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                    {
                        string json = JsonConvert.SerializeObject(new
                        {
                            type = "PlayerConversation",
                            data = new
                            {
                                playerId = this.character.Id,
                                playerMessage = text,
                                targetCharacterId = this.targetCharacter.Id,
                                time = Time.time
                            }
                        }, Formatting.None);

                        WebSocketClient.Instance.websocket.SendText(json);
                    } else
                    {
                        Debug.Log("callback worked but websocket not open");
                    }

                    character.SpeechIcon.enabled = false;
                });
            }
            else
            {
                float directionToTarget = targetCharacter.transform.position.x - character.transform.position.x;
                if (directionToTarget > 0 && character.transform.localScale.x < 0)
                {
                    character.Flip(1);
                }
                else if (directionToTarget < 0 && character.transform.localScale.x > 0)
                {
                    character.Flip(-1);
                }

                if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        type = "StartConversation",
                        data = new
                        {
                            characterId = this.character.Id,
                            targetCharacterId = this.targetCharacter.Id,
                            conversationGoal = this.conversationGoal,
                            time = Time.time
                        }
                    }, Formatting.None);

                    WebSocketClient.Instance.websocket.SendText(json);
                }
            }
        }
       else if (this.targetCharacter == character)
       {
            if (this.targetCharacter.IsPlayerControlled)
            {
                // What do I do if the player is a recipent of a conversation??????

            } else
            {
                // Reminder: this.character and character are different.. this.character is the starter of the conversation and character is who's currently exceuting
                // this action.
                float directionToTarget = this.character.transform.position.x - targetCharacter.transform.position.x;
                Debug.Log($"Target character direction to target: {directionToTarget}");
                if (directionToTarget > 0 && targetCharacter.transform.localScale.x < 0)
                {
                    Debug.Log($"Target character flipping ${1}");
                    targetCharacter.Flip(1);
                }
                else if (directionToTarget < 0 && targetCharacter.transform.localScale.x > 0)
                {
                    Debug.Log($"Target character flipping ${-1}");
                    targetCharacter.Flip(-1);
                }

                if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        type = "Observation",
                        data = new
                        {
                            observerId = this.targetCharacter.Id,
                            observation = $"{this.character.Name} started a conversation with me.",
                            time = Time.time
                        }
                    }, Formatting.None);

                    WebSocketClient.Instance.websocket.SendText(json);
                }
            }
            
        }
    }

    public override void Update(Character character)
    {
        // TODO: add loop in here that basically checks if character is a player, and instead of displaying text, request more text from user?
        if (conversationEvents.Count > 0 && conversationEvents.Peek().characterId == character.Id && this.currentSpeaker == null)
        {
            Debug.Log($"Starting conversation bubble for {character.Name}");
            this.currentSpeaker = character;

            string content = conversationEvents.Peek().content;

            if (DialogueUIManager.Instance.activeConversation == this)
            {
                DialogueUIManager.Instance.DisplayDialogueBox();
                DialogueUIManager.Instance.SetDialogueDisplay(character.Name, "");
            }

            character.StartCoroutine(ProcessDialogue(content));
        }
        else if (conversationEvents.Count <= 0 && conversationFinished)
        {
            DialogueUIManager.Instance.Clear();
            character.ActionQueue.Clear();
            character.FinishAction();
        } else if (conversationEvents.Count <= 0 && character.IsPlayerControlled && shouldPlayerSpeak)
        {
            shouldPlayerSpeak = false;
            character.SpeechIcon.enabled = true;

            DialogueUIManager.Instance.SetDialogueDisplay(this.character.Name, "");
            DialogueInputHandler.Instance.Clear();

            DialogueInputHandler.Instance.GetInputFromPlayer((string text) => {
                DialogueUIManager.Instance.SetDialogueDisplay(this.character.Name, text);

                if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        type = "PlayerConversation",
                        data = new
                        {
                            playerId = this.character.Id,
                            playerMessage = text,
                            targetCharacterId = this.targetCharacter.Id,
                            time = Time.time
                        }
                    }, Formatting.None);

                    WebSocketClient.Instance.websocket.SendText(json);
                }
                else
                {
                    Debug.Log("callback worked but websocket not open");
                }

                character.SpeechIcon.enabled = false;     
            });
        }
    }

    private IEnumerator ProcessDialogue(string dialogueText)
    {
        currentSpeaker.SpeechIcon.enabled = true;

        // Average characters per word is 4.7f for english
        // Speaking WPM is set to 400 (it's a sped up simulation after all, can alter as needed)
        string[] words = dialogueText.Split(' ');

        List<string> parts = new List<string>();

        int i = 0;
        while (i < words.Length)
        {
            string part = "";
            while (i < words.Length && (part + words[i]).Length <= 185)
            {
                part += words[i] + " ";
                i++;
            }

            parts.Add(part);
        }

        foreach (string part in parts)
        {
            currentlyDisplayedText = "";
            foreach (char c in part)
            {
                currentlyDisplayedText += c;

                if (DialogueUIManager.Instance.activeConversation == this)
                {
                    DialogueUIManager.Instance.SetDialogueDisplay(currentSpeaker.Name, currentlyDisplayedText);
                }

                yield return new WaitForSeconds(secondsPerCharacter);
            }
        }

        conversationEvents.Dequeue();

        currentSpeaker.SpeechIcon.enabled = false;

        currentSpeaker = null;

        // Only for player based conversations
        if (character.IsPlayerControlled)
        {
            yield return new WaitForSeconds(3);
            shouldPlayerSpeak = true;
        }
    }

    public override void FixedUpdate(Character character)
    {
       
    }

    public override void Cleanup(Character character)
    { 
        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open && character.Id == this.character.Id)
        {
            Debug.Log($"Sent ActionExecuted for {character.Name}");

            string json = JsonConvert.SerializeObject(new
            {
                type = "ActionExecuted",
                data = new
                {
                    characterId = this.character.Id.ToString(),
                    result = "I spoke with " + targetCharacter.Name + " about " + conversationGoal + ".",
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }

        if (DialogueUIManager.Instance.activeConversation == this)
        {
            DialogueUIManager.Instance.Clear();
        }

        if (character.IsPlayerControlled)
        {
            InventoryUIManager.Instance.gameObject.SetActive(true);
            StatUIManager.Instance.gameObject.SetActive(true);
        }
    }

}
