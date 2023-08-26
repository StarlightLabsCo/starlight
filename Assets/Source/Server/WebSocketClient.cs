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
    public List<Character> characters;
    public Character player;

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

        player = characters.Find(character => character.IsPlayerControlled && character.gameObject.activeInHierarchy);

        // Create a dictionary of id -> character
        foreach (Character c in characters)
        {
            Debug.Log("Adding character " + c.Id + " to dictionary");
            characterDictionary.Add(c.Id, c);
        }

        foreach (Chest c in chests)
        {
            Debug.Log("Adding chest " + c.Id + " to dictionary");
            chestDictionary.Add(c.Id, c);
        }
    }

    // Start is called before the first frame update
    async void Start()
    {
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
                case "move_to":
                    MoveToEvent moveToEvent = jsonObject["data"].ToObject<MoveToEvent>();
                    OnMoveTo(moveToEvent);
                    break;
                case "swing_sword":
                    SwingSwordEvent swingSwordEvent = jsonObject["data"].ToObject<SwingSwordEvent>();
                    OnSwingSword(swingSwordEvent);
                    break;
                case "swing_pickaxe":
                    SwingPickaxeEvent swingPickaxeEvent = jsonObject["data"].ToObject<SwingPickaxeEvent>();
                    OnSwingPickaxe(swingPickaxeEvent);
                    break;
                case "swing_axe":
                    SwingAxeEvent swingAxeEvent = jsonObject["data"].ToObject<SwingAxeEvent>();
                    OnSwingAxe(swingAxeEvent);
                    break;
                case "drop":
                    DropItemEvent dropItemEvent = jsonObject["data"].ToObject<DropItemEvent>();
                    OnDropItem(dropItemEvent);
                    break;
                case "add_to_chest":
                    AddItemToChestEvent addItemToChestEvent = jsonObject["data"].ToObject<AddItemToChestEvent>();
                    OnAddItemToChest(addItemToChestEvent);
                    break;
                case "remove_from_chest":
                    RemoveItemFromChestEvent removeItemFromChestEvent = jsonObject["data"].ToObject<RemoveItemFromChestEvent>();
                    OnRemoveItemFromChest(removeItemFromChestEvent);
                    break;
                case "start_conversation":
                    StartConversationEvent startConversationEvent = jsonObject["data"].ToObject<StartConversationEvent>();
                    OnStartConversation(startConversationEvent);
                    break;
                case "conversation":
                    ConversationEvent conversationEvent = jsonObject["data"].ToObject<ConversationEvent>();
                    OnConversation(conversationEvent);
                    break;
                case "end_conversation":
                    EndConversationEvent endConversationEvent = jsonObject["data"].ToObject<EndConversationEvent>();
                    OnEndConversation(endConversationEvent);
                    break;
                case "eat":
                    EatEvent eatEvent = jsonObject["data"].ToObject<EatEvent>();
                    OnEatEvent(eatEvent);
                    break;
                case "SetWorldTime":
                    SetWorldTimeEvent setWorldTimeEvent = jsonObject["data"].ToObject<SetWorldTimeEvent>();
                    OnSetWorldTime(setWorldTimeEvent);
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
        // If the queue already contains the character, remove it so it can be added to the back once the websocket message is sent.
        if (CameraManager.Instance.cameraFocusQueue.Contains(character))
        {
            Queue<Character> newQueue = new Queue<Character>();

            foreach (Character item in CameraManager.Instance.cameraFocusQueue)
            {
                if (item != character)
                {
                    newQueue.Enqueue(item);
                }
            }

            CameraManager.Instance.cameraFocusQueue = newQueue;

        }

        // Get Character Location
        string characterLocation = "{ \"x\": " + character.gameObject.transform.position.x + ", \"y\": " + character.gameObject.transform.position.y + " }";


        // Get Character Satiety
        float characterSatiety = -1;
        float characterMaxSatiety = -1;
        if (character is IHasStomach)
        {
            characterSatiety = ((IHasStomach)character).Satiety;
            characterMaxSatiety = ((IHasStomach)character).MaxSatiety;
        }


        // Get current character inventory
        string[] inventoryArray = null;
        if (character is IHasInventory)
        {
            IHasInventory characterWithInventory = (IHasInventory)character;
            Inventory characterInventory = characterWithInventory.EntityInventory;

            inventoryArray = characterInventory.AsList().Select(i => i.ToString()).ToArray();
        }

        // Get current available actions from character
        List<Action> availableActions = character.GetAvailableActions();
        string[] actionsArray = new string[availableActions.Count];
        for (int i = 0; i < availableActions.Count; i++)
        {
            Debug.Log(availableActions[i].Name);
            string json = JsonConvert.SerializeObject(new
            {
                name = availableActions[i].Name,
                description = availableActions[i].Description,
                parameters = availableActions[i].Parameters
            }, Formatting.None);
            actionsArray[i] = json;
        }

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
            Debug.Log($"Sending agent loop for {character.Id}");

            // Convert arrays to JSON strings
            string actionsJson = JsonConvert.SerializeObject(actionsArray);
            string environmentJson = JsonConvert.SerializeObject(environmentArray);
            string hitboxJson = JsonConvert.SerializeObject(hitboxStringArray);

            string jsonString = "{ \"type\": \"GetAction\", \"data\": { \"characterId\": \"" + character.Id + "\", \"location\": " + characterLocation + ", \"availableActions\": " + actionsJson + ", \"inventory\": " + JsonConvert.SerializeObject(inventoryArray) + ", \"environment\": " + environmentJson + ", \"hitbox\": " + hitboxJson + ", \"time\": " + Time.time + ", \"satiety\": " + characterSatiety + ", \"maxSatiety\": " + characterMaxSatiety + " } }";

            await websocket.SendText(jsonString);

            CameraManager.Instance.cameraFocusQueue.Enqueue(character);
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
        Debug.Log("Move to event received for character " + moveToEvent.characterId + " at X: " + moveToEvent.x + ", Y: " + moveToEvent.y);
        try
        {
            characterDictionary[moveToEvent.characterId].ActionQueue.Enqueue(new MoveTo(new Vector2(moveToEvent.x, moveToEvent.y)));
            characterDictionary[moveToEvent.characterId].IsRequestingAction = false;
        }
        catch (Exception e)
        {
            // Retry
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[moveToEvent.characterId]);
        }
    }

    public void OnSwingAxe(SwingAxeEvent swingAxeEvent)
    {
        try
        {
            characterDictionary[swingAxeEvent.characterId].ActionQueue.Enqueue(new SwingAxe());
            characterDictionary[swingAxeEvent.characterId].IsRequestingAction = false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[swingAxeEvent.characterId]);
        }
    }

    public void OnSwingSword(SwingSwordEvent swingSwordEvent)
    {
        try
        {
            characterDictionary[swingSwordEvent.characterId].ActionQueue.Enqueue(new SwingSword());
            characterDictionary[swingSwordEvent.characterId].IsRequestingAction = false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[swingSwordEvent.characterId]);
        }
    }

    public void OnSwingPickaxe(SwingPickaxeEvent swingPickaxeEvent)
    {
        try
        {
            characterDictionary[swingPickaxeEvent.characterId].ActionQueue.Enqueue(new SwingPickaxe());
            characterDictionary[swingPickaxeEvent.characterId].IsRequestingAction = false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[swingPickaxeEvent.characterId]);
        }
    }

    public void OnDropItem(DropItemEvent dropItemEvent)
    {
        try
        {
            DropItem dropItem = new(Utilities.idToItem(dropItemEvent.itemId));
            characterDictionary[dropItemEvent.characterId].ActionQueue.Enqueue(dropItem);
            characterDictionary[dropItemEvent.characterId].IsRequestingAction = false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[dropItemEvent.characterId]);
        }
    }

    public void OnAddItemToChest(AddItemToChestEvent addItemToChestEvent)
    {
        try
        {
            Debug.Log("Adding item to chest " + addItemToChestEvent.chestId + " from character " + addItemToChestEvent.characterId + " with item " + addItemToChestEvent.itemId);
            Debug.Log("Chest dictionary: " + chestDictionary);
            Debug.Log("Chest dictionary result: " + chestDictionary[addItemToChestEvent.chestId]);
            Debug.Log("Item mapping: " + Utilities.idToItem(addItemToChestEvent.itemId));

            AddItemToChest addItemToChest = new AddItemToChest(chestDictionary[addItemToChestEvent.chestId], Utilities.idToItem(addItemToChestEvent.itemId));
            characterDictionary[addItemToChestEvent.characterId].ActionQueue.Enqueue(addItemToChest);
            characterDictionary[addItemToChestEvent.characterId].IsRequestingAction = false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[addItemToChestEvent.characterId]);
        }
    }

    public void OnRemoveItemFromChest(RemoveItemFromChestEvent removeItemFromChestEvent)
    {
        try
        {
            RemoveItemFromChest removeItemFromChest = new(chestDictionary[removeItemFromChestEvent.chestId], Utilities.idToItem(removeItemFromChestEvent.itemId));
            characterDictionary[removeItemFromChestEvent.characterId].ActionQueue.Enqueue(removeItemFromChest);
            characterDictionary[removeItemFromChestEvent.characterId].IsRequestingAction = false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[removeItemFromChestEvent.characterId]);
        }
    }

    public void OnStartConversation(StartConversationEvent startConversationEvent)
    {
         try
        {
            StartConversation startConversation = new((Human)characterDictionary[startConversationEvent.characterId], (Human)characterDictionary[startConversationEvent.targetCharacterId], startConversationEvent.conversationGoal);

            characterDictionary[startConversationEvent.characterId].ActionQueue.Enqueue(startConversation);
            characterDictionary[startConversationEvent.characterId].IsRequestingAction = false;

            characterDictionary[startConversationEvent.targetCharacterId].ActionQueue.Clear();
            characterDictionary[startConversationEvent.targetCharacterId].ActionQueue.Enqueue(startConversation);
            characterDictionary[startConversationEvent.targetCharacterId].IsRequestingAction = false;
        } catch (Exception e)
        {
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[startConversationEvent.characterId]);
        }
    }

    public void OnConversation(ConversationEvent conversationEvent)
    {
        Debug.Log("On Conversation");
        try
        {
            Character character = characterDictionary[conversationEvent.characterId];

            if (character.CurrentAction is StartConversation)
            {
                Debug.Log($"Added conversation event to StartConversation action");
                ((StartConversation)character.CurrentAction).conversationEvents.Enqueue(conversationEvent);
            }
        } catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OnEndConversation(EndConversationEvent endConversationEvent)
    {
        try
        {
            Debug.Log("On End Conversation");

            Character character = characterDictionary[endConversationEvent.characterId];
            Action action = character.CurrentAction;

            if (action is StartConversation)
            {
                ((StartConversation)action).conversationFinished = true;
            }
        } catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OnEatEvent(EatEvent eatEvent)
    {
        try
        {
            Character character = characterDictionary[eatEvent.characterId];
            EatableItem food = (EatableItem)Utilities.idToItem(eatEvent.foodId);

            character.ActionQueue.Enqueue(new Eat(food));
            character.IsRequestingAction = false;
        } catch(Exception e)
        {
            Debug.LogError(e);
            SendWebSocketMessage(characterDictionary[eatEvent.characterId]);
        }
    }

    public void OnSetWorldTime(SetWorldTimeEvent setWorldTimeEvent)
    {
        try
        {
            WorldTime.Instance.SetTime(setWorldTimeEvent.time);
        }
        catch (Exception e)
        {
            Debug.Log("Error setting world time: " + e);
        }
    }
}