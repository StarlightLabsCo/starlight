using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NativeWebSocket;
using WebSocketEvents;
using System.Linq;

public class WebSocketClient : MonoBehaviour
{
    public WebSocket websocket;

    public static WebSocketClient Instance { get; private set; }

    // Characters
    public List<Character> character;
    private Dictionary<string, Character> characterDictionary = new Dictionary<string, Character>();

    // Chests
    public List<Chest> chests;
    private Dictionary<string, Chest> chestDictionary = new Dictionary<string, Chest>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    async void Start()
    {
        // Create a dictionary of id -> character
        foreach (Character c in character)
        {
            Debug.Log("Adding character " + c.characterId + " to dictionary");
            characterDictionary.Add(c.characterId, c);
        }

        foreach (Chest c in chests)
        {
            Debug.Log("Adding chest " + c.Id + " to dictionary");
            chestDictionary.Add(c.Id, c);
        }

        // Set up the websocket
        websocket = new WebSocket("ws://localhost:8082");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // parse the message into a JSON object
            string message = System.Text.Encoding.UTF8.GetString(bytes);

            Debug.Log("OnMessage! " + message);

            // Deserialize the message into JObject
            var jsonObject = JsonConvert.DeserializeObject<JObject>(message);
            var eventType = jsonObject["type"].Value<string>();

            switch (eventType)
            {
                case "ConnectionEstablished":
                    Debug.Log("Connection established!");
                    break;
                case "MoveTo":
                    MoveToEvent moveToEvent = jsonObject["data"].ToObject<MoveToEvent>();
                    OnMoveTo(moveToEvent);
                    break;
                case "SwingSword":
                    SwingSwordEvent swingSwordEvent = jsonObject["data"].ToObject<SwingSwordEvent>();
                    OnSwingSword(swingSwordEvent);
                    break;
                case "SwingPickaxe":
                    SwingPickaxeEvent swingPickaxeEvent = jsonObject["data"].ToObject<SwingPickaxeEvent>();
                    OnSwingPickaxe(swingPickaxeEvent);
                    break;
                case "SwingAxe":
                    SwingAxeEvent swingAxeEvent = jsonObject["data"].ToObject<SwingAxeEvent>();
                    OnSwingAxe(swingAxeEvent);
                    break;
                case "DropItem":
                    DropItemEvent dropItemEvent = jsonObject["data"].ToObject<DropItemEvent>();
                    OnDropItem(dropItemEvent);
                    break;
                case "AddItemToChest":
                    AddItemToChestEvent addItemToChestEvent = jsonObject["data"].ToObject<AddItemToChestEvent>();
                    OnAddItemToChest(addItemToChestEvent);
                    break;
                case "RemoveItemFromChest":
                    RemoveItemFromChestEvent removeItemFromChestEvent = jsonObject["data"].ToObject<RemoveItemFromChestEvent>();
                    OnRemoveItemFromChest(removeItemFromChestEvent);
                    break;
                default:
                    // Try again
                    Debug.Log("Unknown event type: " + eventType);
                    SendWebSocketMessage(characterDictionary["A1"]);
                    break;
            }
        };

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }
    public async void SendWebSocketMessage(Character character)
    {
        string characterLocation = "{ \"x\": " + character.gameObject.transform.position.x + ", \"y\": " + character.gameObject.transform.position.y + " }";

        // Get current character inventory
        string[] inventoryArray = null;
        if (character is IHasInventory)
        {
            IHasInventory characterWithInventory = (IHasInventory)character;
            Inventory characterInventory = characterWithInventory.EntityInventory;

            inventoryArray = characterInventory.Items.Select(i => i.ToString()).ToArray();
        }

        // Get current available actions from character
        List<Action> availableActions = character.GetAvailableActions();

        // Convert list to array of string representations
        string[] actionsArray = availableActions.Select(a => a.ToString()).ToArray();

        // Get environment (list of interactables in the vicinity of the character)
        string[] environmentArray = character.GetEnvironment();

        // Get what's in the player's hitbox
        Vector2 offset = new Vector2(character.transform.localScale.x, 0);
        Vector2 size = new Vector2(1.2f, 1f);

        Collider2D[] collisions = Utilities.DetectCollisions(character, offset, size, LayerMask.GetMask("Default"));

        List<string> hitboxArray = new List<string>();
        foreach (Collider2D collision in collisions)
        {
            if (collision.gameObject.GetComponent<Entity>() != null && collision.gameObject.GetComponent<Entity>() != character)
            {
                Entity entity = collision.gameObject.GetComponent<Entity>();
                hitboxArray.Add(collision.gameObject.GetComponent<Entity>().Name + " (Health: " + entity.Health + "/" + entity.MaxHealth + ")");
            }
        }

        string[] hitboxStringArray = hitboxArray.ToArray();


        if (websocket.State == WebSocketState.Open)
        {
            Debug.Log("Sending agent loop");

            // Convert arrays to JSON strings
            string actionsJson = JsonConvert.SerializeObject(actionsArray);
            string environmentJson = JsonConvert.SerializeObject(environmentArray);
            string hitboxJson = JsonConvert.SerializeObject(hitboxStringArray);

            string jsonString = "{ \"type\": \"GetAction\", \"data\": { \"characterId\": \"" + character.Id + "\", \"location\": " + characterLocation + ", \"availableActions\": " + actionsJson + ", \"inventory\": " + JsonConvert.SerializeObject(inventoryArray) + ", \"environment\": " + environmentJson + ", \"hitbox\": " + hitboxJson + " } }";

            await websocket.SendText(jsonString);
        }
        else
        {
            Debug.Log("Websocket not open");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    // Event handlers
    public void OnMoveTo(MoveToEvent moveToEvent)
    {
        characterDictionary[moveToEvent.characterId].ActionQueue.Enqueue(new MoveTo(moveToEvent.x, moveToEvent.y));
        characterDictionary[moveToEvent.characterId].IsRequestingAction = false;
    }

    public void OnSwingAxe(SwingAxeEvent swingAxeEvent)
    {
        characterDictionary[swingAxeEvent.characterId].ActionQueue.Enqueue(new SwingAxe());
        characterDictionary[swingAxeEvent.characterId].IsRequestingAction = false;
    }

    public void OnSwingSword(SwingSwordEvent swingSwordEvent)
    {
        characterDictionary[swingSwordEvent.characterId].ActionQueue.Enqueue(new SwingSword());
        characterDictionary[swingSwordEvent.characterId].IsRequestingAction = false;
    }

    public void OnSwingPickaxe(SwingPickaxeEvent swingPickaxeEvent)
    {
        characterDictionary[swingPickaxeEvent.characterId].ActionQueue.Enqueue(new SwingPickaxe());
        characterDictionary[swingPickaxeEvent.characterId].IsRequestingAction = false;
    }

    public void OnDropItem(DropItemEvent dropItemEvent)
    {
        DropItem dropItem = new(Utilities.idToItem(dropItemEvent.itemId));
        characterDictionary[dropItemEvent.characterId].ActionQueue.Enqueue(dropItem);
        characterDictionary[dropItemEvent.characterId].IsRequestingAction = false;
    }

    public void OnAddItemToChest(AddItemToChestEvent addItemToChestEvent)
    {
        Debug.Log("Adding item to chest " + addItemToChestEvent.chestId + " from character " + addItemToChestEvent.characterId + " with item " + addItemToChestEvent.itemId);
        Debug.Log("Chest dictionary: " + chestDictionary);
        Debug.Log("Chest dictionary result: " + chestDictionary[addItemToChestEvent.chestId]);
        Debug.Log("Item mapping: " + Utilities.idToItem(addItemToChestEvent.itemId));

        AddItemToChest addItemToChest = new AddItemToChest(chestDictionary[addItemToChestEvent.chestId], Utilities.idToItem(addItemToChestEvent.itemId));
        characterDictionary[addItemToChestEvent.characterId].ActionQueue.Enqueue(addItemToChest);
        characterDictionary[addItemToChestEvent.characterId].IsRequestingAction = false;
    }

    public void OnRemoveItemFromChest(RemoveItemFromChestEvent removeItemFromChestEvent)
    {
        RemoveItemFromChest removeItemFromChest = new(chestDictionary[removeItemFromChestEvent.chestId], Utilities.idToItem(removeItemFromChestEvent.itemId));
        characterDictionary[removeItemFromChestEvent.characterId].ActionQueue.Enqueue(removeItemFromChest);
        characterDictionary[removeItemFromChestEvent.characterId].IsRequestingAction = false;
    }

}