using System.Collections;
using System.Collections.Generic;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketEvents;

public class StartConversation : Action
{

    Character character;
    Character targetCharacter;
    string conversationGoal;
    Character currentSpeaker = null;

    public Queue<ConversationEvent> conversationEvents = new Queue<ConversationEvent>();

    public bool conversationFinished = false;

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
       if (this.character == character)
       {
            float directionToTarget = targetCharacter.transform.position.x - character.transform.position.x;
            if (directionToTarget > 0 && character.transform.localScale.x < 0)
            {
                character.Flip(1);
            } else if (directionToTarget < 0 && character.transform.localScale.x > 0)
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
       else if (this.targetCharacter == character)
       {
            float directionToTarget = character.transform.position.x - targetCharacter.transform.position.x;
            if (directionToTarget > 0 && targetCharacter.transform.localScale.x < 0)
            {
                targetCharacter.Flip(1);
            }
            else if (directionToTarget < 0 && targetCharacter.transform.localScale.x > 0)
            {
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

    public override void Update(Character character)
    {
        if (conversationEvents.Count > 0 && conversationEvents.Peek().characterId == character.Id && this.currentSpeaker == null)
        {
            Debug.Log($"Starting conversation bubble for {character.Name}");
            this.currentSpeaker = character;

            string content = conversationEvents.Peek().content;

            character.SpeechIcon.enabled = true;
            DialogueManager.Instance.DisplayDialogue(character.Name, content, () =>finishSpeaking(character));
        }
        else if (conversationEvents.Count <= 0 && conversationFinished)
        {
            Debug.Log($"Finishing action for ${character.Name}");
            DialogueManager.Instance.Clear();
            character.ActionQueue.Clear();
            character.FinishAction();
        }
    }

    private void finishSpeaking(Character character)
    {
        conversationEvents.Dequeue();

        character.SpeechIcon.enabled = false;

        this.currentSpeaker = null;
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
    }

}
