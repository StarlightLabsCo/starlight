using System;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using static Utilities;

public class AddItemToChest : Action
{
    private Chest chest;
    private Item item;

    public AddItemToChest(Chest chest, Item item) : base($"add_{item.Id}_to_chest", $"add_{item.Id}_to_chest", $"Add ${item.Name} to chest {chest.Id}.", JsonConvert.SerializeObject(new
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
        return $"[AddItemToChest] Add {item.Name} to chest.\n" + "-- e.g. { \"type\": \"AddItemToChest\", \"data\": { \"characterId\": \"A1\", \"chestId\": \"" + chest.Id + "\", \"itemId\": \"" + item.Id + "\" }}";
    }

    public override void Execute(Character character)
    {
        Debug.Log("Adding " + item.Name + " to chest " + chest.Id + " at X: " + chest.transform.position.x + ", Y: " + chest.transform.position.y);

        if (character is IHasInventory)
        {
            Debug.Log("Character has inventory");

            bool success = chest.AddItem(item);

            if (success)
            {
                Debug.Log("Added " + item.Name + " to chest");

                if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        type = "Observation",
                        data = new
                        {
                            observerId = character.Id.ToString(),
                            observation = character.Name + " deposited " + item.Name + " to chest (" + chest.Id + ") at X: " + chest.transform.position.x + ", Y: " + chest.transform.position.y,
                            time = Time.time
                        }
                    }, Formatting.None);

                    WebSocketClient.Instance.websocket.SendText(json);
                }

                bool removed = (character as IHasInventory).EntityInventory.Remove(item);

                if (removed)
                {
                    character.PlayRemoveItemAnimation(item.sprite);
                }
                else
                {
                    Debug.LogError("Failed to remove " + item.Name + " from character");
                }
            }
            else
            {
                Debug.LogError("Failed to add " + item.Name + " to chest");
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
                    result = character.Name + " deposited " + item.Name + " into chest " + chest.Id + " at X: " + chest.transform.position.x + ", Y: " + chest.transform.position.y + ".",
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }
    }
}