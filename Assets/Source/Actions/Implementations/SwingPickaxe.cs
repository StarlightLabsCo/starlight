using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class SwingPickaxe : AnimationAction
{
    int hits = 0;

    public SwingPickaxe() : base("swing_pickaxe", "swing_pickaxe", "Swing Pickaxe.", JsonConvert.SerializeObject(new
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
        EnergyCost = 10;
    }

    public override string ToString()
    {
        return "[SwingPickaxe] Swing pickaxe. \n-- e.g. {\"type\": \"SwingPickaxe\", \"data\": {\"characterId\": \"A1\"}}";
    }

    public override void Execute(Character character)
    {
        if (character.Energy - EnergyCost <= 0)
        {
            Debug.Log("Not enough energy, unable to execute!");

            character.ClearAction();
            return;
        }

        // Play animation
        character.DecreaseEnergy(EnergyCost);
        character.PlayAnimation("mining");
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

        Collider2D[] collisions = Utilities.DetectCollisions(character, offset, size, LayerMask.GetMask("Obstacles"));

        // If so deal damage
        foreach (Collider2D collision in collisions)
        {
            if (collision.gameObject.GetComponent<MineableEntity>() != null)
            {
                hits++;
                collision.gameObject.GetComponent<MineableEntity>().TakeDamage(5);
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
                    result = character.Name + " swong their pickaxe at X: " + character.transform.position.x + ", Y: " + character.transform.position.y + " and hit " + hits + " mineable entities.",
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }

        character.PlayAnimation("idle");
    }
}