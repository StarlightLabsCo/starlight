using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class SwingAxe : AnimationAction
{
    int hits = 0;

    public SwingAxe() : base(System.Guid.NewGuid().ToString(), "SwingAxe", "Swing Axe")
    {

    }


    public SwingAxe(string id, string name, string description) : base(id, name, description)
    {

    }

    public override string ToString()
    {
        return "[SwingAxe] Swing axe. \n-- e.g. {\"type\": \"SwingAxe\", \"data\": {\"characterId\": \"A1\"}}";
    }

    public override void Execute(Character character)
    {
        // Play animation
        Debug.Log("[1] SwingAxe execute");

        character.PlayAnimation("axe");
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
        Debug.Log("[1] SwingAxe trigger effect");

        // Define offset and size as Vector2
        Vector2 offset = new Vector2(character.transform.localScale.x, 0);
        Vector2 size = new Vector2(1.2f, 1f);

        Collider2D[] collisions = Utilities.DetectCollisions(character, offset, size, LayerMask.GetMask("Default"));

        // If so deal damage
        foreach (Collider2D collision in collisions)
        {
            if (collision.gameObject.GetComponent<ChoppableEntity>() != null)
            {
                Debug.Log("Tree hit");
                hits++;
                collision.gameObject.GetComponent<ChoppableEntity>().TakeDamage(5);
            }
        }
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
                    result = character.Name + " swong their axe at X: " + character.transform.position.x + ", Y: " + character.transform.position.y + " and hit " + hits + " trees."
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }

        character.PlayAnimation("idle");
    }
}