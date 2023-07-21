using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class AddItemToChest : Action
{
    private Chest chest;
    private Item item;

    public AddItemToChest(Chest chest, Item item) : base("add_item_to_chest", "Add Item To Chest", "Add an item to a chest")
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
                            observation = character.Name + " deposited " + item.Name + " to chest (" + chest.Id + ") at X: " + chest.transform.position.x + ", Y: " + chest.transform.position.y
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
                    result = character.Name + " deposited " + item.Name + " into chest " + chest.Id + " at X: " + chest.transform.position.x + ", Y: " + chest.transform.position.y + "."
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }
    }
}