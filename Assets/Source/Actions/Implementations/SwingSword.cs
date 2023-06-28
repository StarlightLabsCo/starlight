using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class SwingSword : AnimationAction
{
    public SwingSword() : base(System.Guid.NewGuid().ToString(), "SwingSword", "Swing sword")
    {

    }


    public SwingSword(string id, string name, string description) : base(id, name, description)
    {

    }

    public override string ToString()
    {
        return $"[SwingSword] Swing sword.";
    }

    public override void Execute(Character character)
    {
        // Play animation
        character.PlayAnimation("attack");
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
        // Define offset and size as Vector2
        Vector2 offset = new Vector2(character.transform.localScale.x, 0);
        Vector2 size = new Vector2(1.2f, 1f);

        Collider2D[] collisions = Utilities.DetectCollisions(character, offset, size, LayerMask.GetMask("Default"));

        // If so deal damage
        int hits = 0;
        foreach (Collider2D collision in collisions)
        {
            if (collision.gameObject.GetComponent<Character>() != null)
            {
                hits++;
                collision.gameObject.GetComponent<Character>().TakeDamage(10);
            }
        }

        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
        {
            string json = JsonConvert.SerializeObject(new
            {
                type = "Observation",
                data = new
                {
                    observerId = character.Id.ToString(),
                    observation = character.Name + " swong a sword at X: " + character.transform.position.x + ", Y: " + character.transform.position.y + " and hit " + hits + " characters."
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }

    }

    public override void Cleanup(Character character)
    {
        character.PlayAnimation("idle");
    }
}