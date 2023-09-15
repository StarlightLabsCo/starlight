using System;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class Sleep : Action
{

    House targetHouse;

    public Sleep(House targetHouse) : base("sleep", "sleep", $"Sleep in house (Id: {targetHouse.Id}).", JsonConvert.SerializeObject(new
    {
        type = "object",
        properties = new
        {
            characterId = new
            {
                type = "string",
                description = "The character ID of the character that going to sleep."
            },
            targetHouseId = new
            {
                type = "string",
                description = "The house ID of the target house the character wants to sleep in."
            }
        }
    }))
    {
        this.targetHouse = targetHouse;
    }

    // Start the action
    public override void Execute(Character character)
    {
        character.gameObject.SetActive(false);
        targetHouse.Occupy(character);
    }

    // Update loop for the character 
    public override void Update(Character character)
    {
    }

    // Fixed update loop for the character - for physics, etc
    public override void FixedUpdate(Character character)
    {

    }

    public override void Cleanup(Character character)
    {
        targetHouse.deoccupy();

        character.Energy = character.MaxEnergy;

        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
        {
            string json = JsonConvert.SerializeObject(new
            {
                type = "ActionExecuted",
                data = new
                {
                    characterId = character.Id.ToString(),
                    result = $"{character.Name} finished sleeping.",
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }
    }
}
