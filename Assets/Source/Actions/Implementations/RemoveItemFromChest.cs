using System;
using System.Collections.Generic;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using static Utilities;

public class RemoveItemFromChest : Action
{
    private Chest chest;
    private Item item;

    public RemoveItemFromChest(Chest chest, Item item) : base($"remove_{item.Id}_from_chest", $"remove_{item.Id}_from_chest", $"Remove {item.Name} from a chest {chest.Id}.", JsonConvert.SerializeObject(new
    {
        type = "object",
        properties = new
        {
            characterId = new
            {
                type = "string",
                description = "The character ID of the character that is adding the item to the chest."
            },
            chestId = new
            {
                type = "string",
                description = "The chest ID of the chest that the item is being added to."
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
        this.chest = chest;
        this.item = item;
    }

    public override string ToString()
    {
        return $"[RemoveItemFromChest] Remove {item.Name} from chest and add to inventory.\n" + "-- e.g. { \"type\": \"RemoveItemFromChest\", \"data\": { \"characterId\": \"A1\", \"chestId\": \"" + chest.Id + "\", \"itemId\": \"" + item.Id + "\" }}";
    }

    public override void Cleanup(Character character)
    {
        return;
    }

    public override void Execute(Character character)
    {
        if (character is IHasInventory)
        {
            bool success = chest.RemoveItem(item);

            if (success)
            {

                if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        type = "Observation",
                        data = new
                        {
                            observerId = character.Id.ToString(),
                            observation = character.Name + " withdrew " + item.Name + " from chest (" + chest.Id + ") at X: " + chest.transform.position.x + ", Y: " + chest.transform.position.y,
                            time = Time.time
                        }
                    }, Formatting.None);

                    WebSocketClient.Instance.websocket.SendText(json);
                }

                (character as IHasInventory).EntityInventory.Add(item);
                Debug.Log("Removed " + item.Name + " from chest");
            }
            else
            {
                Debug.Log("Failed to remove " + item.Name + " from chest");
            }
        }

        character.FinishAction();
    }

    public override void FixedUpdate(Character character)
    {
        return;
    }

    public override void Update(Character character)
    {
        return;
    }
}