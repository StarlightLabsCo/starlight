using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class SwingHammer : AnimationAction
{
    int hits = 0;

    public SwingHammer() : base(System.Guid.NewGuid().ToString(), "SwingHammer", "Swing Hammer")
    {

    }


    public SwingHammer(string id, string name, string description) : base(id, name, description)
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
                    result = character.Name + " swong their hammer at X: " + character.transform.position.x + ", Y: " + character.transform.position.y
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }

        character.PlayAnimation("idle");
    }
}