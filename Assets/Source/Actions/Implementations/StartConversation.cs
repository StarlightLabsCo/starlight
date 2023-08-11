using System.Collections;
using System.Collections.Generic;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class StartConversation : Action
{

    Human character;
    Human targetCharacter;
    string conversationGoal;
    private Human currentlySpeaking;

    public StartConversation(Human character, Human targetCharacter, string conversationGoal) : base("start_conversation", "start_conversation", $"Start a conversation with {targetCharacter.Name} (ID: {targetCharacter.Id}).", JsonConvert.SerializeObject(new
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
        // TODO: pathfind characters to each other so they can talk?

       if (this.character == character)
       {
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
      // process conversation queue accordingly
    }

    public override void FixedUpdate(Character character)
    {
       
    }

    public override void Cleanup(Character character)
    {
        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
        {
            string json = JsonConvert.SerializeObject(new
            {
                type = "ActionExecuted",
                data = new
                {
                    characterId = character.Id.ToString(),
                    result = character.Name + " spoke with " + targetCharacter.Name + " about " + conversationGoal + ".",
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }
    }

}
