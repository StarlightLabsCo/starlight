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
    WebSocket websocket;

    public static WebSocketClient Instance { get; private set; }

    // Characters
    public List<Character> character;
    private Dictionary<string, Character> characterDictionary = new Dictionary<string, Character>();

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
    void Start()
    {
        // Create a dictionary of id -> character
        foreach (Character c in character)
        {
            Debug.Log("Adding character " + c.characterId + " to dictionary");
            characterDictionary.Add(c.characterId, c);
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
                default:
                    throw new Exception("Unknown event type: " + eventType);
            }
        };

        // waiting for messages
        websocket.Connect();

        InvokeRepeating("SendWebSocketMessage", 2.0f, 7f);
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }
    public async void SendWebSocketMessage()
    {
        string characterId = "1";

        Character character = characterDictionary[characterId];

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
                hitboxArray.Add(collision.gameObject.GetComponent<Entity>().Name);
            }
            else if (collision.gameObject.GetComponent<ItemDisplay>() != null)
            {
                hitboxArray.Add(collision.gameObject.GetComponent<ItemDisplay>().item.Name);
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

            string jsonString = "{ \"type\": \"GetAction\", \"data\": { \"characterId\": \"1\", \"location\": " + characterLocation + ", \"availableActions\": " + actionsJson + ", \"inventory\": " + JsonConvert.SerializeObject(inventoryArray) + ", \"environment\": " + environmentJson + ", \"hitbox\": " + hitboxJson + " } }";


            await websocket.SendText(jsonString);
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
    }

    public void OnSwingAxe(SwingAxeEvent swingAxeEvent)
    {
        characterDictionary[swingAxeEvent.characterId].ActionQueue.Enqueue(new SwingAxe());
    }

    public void OnSwingSword(SwingSwordEvent swingSwordEvent)
    {
        characterDictionary[swingSwordEvent.characterId].ActionQueue.Enqueue(new SwingSword());
    }

    public void OnSwingPickaxe(SwingPickaxeEvent swingPickaxeEvent)
    {
        characterDictionary[swingPickaxeEvent.characterId].ActionQueue.Enqueue(new SwingPickaxe());
    }

}