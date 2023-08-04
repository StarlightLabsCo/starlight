using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class SwingHammer : AnimationAction
{
    int hits = 0;

    public SwingHammer() : base("swing_hammer", "swing_hammer", "Swing Hammer.", JsonConvert.SerializeObject(new
    {
        type = "object",
        properties = new
        {
            characterId = new
            {
                type = "string",
                description = "The character ID of the character that is adding the item to the chest."
            },
        }
    }))
    {

    }

    public override string ToString()
    {
        return $"[SwingHammer] Swing hammer.";
    }

    public override void Execute(Character character)
    {
        // Play animation
        character.PlayAnimation("hammering");
    }

    public override void Update(Character character)
    {
        return;
    }

    public override void FixedUpdate(Character character)
    {
        return;
    }

    public override void TriggerEffect(Character character)
    {
        // Create hitbox in front of character on certain frame
        Debug.Log("Hammer damage would apply now");

        // Check if hitbox collides with enemy

        // If so deal damage


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
                    result = character.Name + " swong their hammer at X: " + character.transform.position.x + ", Y: " + character.transform.position.y,
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }

        character.PlayAnimation("idle");
    }
}