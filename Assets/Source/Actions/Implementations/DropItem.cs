using System;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using static Utilities;
using Random = UnityEngine.Random;

public class DropItem : Action
{
    Item item;

    public DropItem(Item item) : base($"drop_{item.Id}", $"drop_{item.Id}", $"Drop {item.Name} onto the ground.", JsonConvert.SerializeObject(new
    {
        type = "object",
        properties = new
        {
            characterId = new
            {
                type = "string",
                description = "The character ID of the character that is adding the item to the chest."
            },
            itemId = new
            {
                type = "string",
                description = "The item ID of the item that is being added to the chest. Must be lowercase.",
                values = Enum.GetNames(typeof(ItemId))
            }
        }
    }))
    {
        this.item = item;
    }

    public override string ToString()
    {
        return $"[DropItem] Drop {item.Name} from inventory to ground.\n-- e.g. " + "{\"type\": \"DropItem\", \"data\": {\"characterId\": \"A1\", \"itemId\": \"" + item.Id + "\"}}";
    }

    public override void Execute(Character character)
    {
        if (character is IHasInventory)
        {
            Debug.Log("Dropping item " + item.Name + " from inventory");
            Debug.Log("Inventory before: ");
            foreach (Item i in (character as IHasInventory).EntityInventory.Items)
            {
                Debug.Log("- " + i.Name);
            }

            IHasInventory inventory = character as IHasInventory;
            bool wasRemoved = inventory.EntityInventory.Remove(item);
            if (wasRemoved)
            {
                Debug.Log("Item removed from inventory");

                character.PlayRemoveItemAnimation(item.sprite);

                // Create a new item display and place it at the character's position
                GameObject itemDisplay = GameObject.Instantiate(Resources.Load<GameObject>("Item"));
                itemDisplay.GetComponent<ItemDisplay>().item = item;
                itemDisplay.transform.position = character.transform.position;

                // Apply a slight force so it moves away from the player in the direction they are facing
                Rigidbody2D rb = itemDisplay.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float force = 5f; // Change this value to control the amount of force
                    Vector2 direction = character.transform.localScale.x > 0 ? Vector2.right : Vector2.left; // Get the direction the character is facing

                    // Add a slight random bias to the direction
                    float randomBias = Random.Range(-0.2f, 0.2f); // Change these values to control the amount of bias
                    direction = new Vector2(direction.x + randomBias, direction.y + randomBias).normalized;

                    rb.AddForce(direction * force, ForceMode2D.Impulse);
                }

                if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        type = "Observation",
                        data = new
                        {
                            observerId = character.Id.ToString(),
                            observation = character.Name + " dropped " + item.Name + " at X: " + rb.transform.position.x + ", Y: " + rb.transform.position.y,
                            time = Time.time
                        }
                    }, Formatting.None);

                    WebSocketClient.Instance.websocket.SendText(json);
                }
            }
            else
            {
                Debug.Log("Item not found in inventory");
            }

            character.FinishAction();
        }
    }

    public override void FixedUpdate(Character character)
    {
        return;
    }

    public override void Update(Character character)
    {
        return;
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
                    result = character.Name + " dropped " + item.Name + " on the ground at X: " + character.transform.position.x + ", Y: " + character.transform.position.y + ".",
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }
    }
}