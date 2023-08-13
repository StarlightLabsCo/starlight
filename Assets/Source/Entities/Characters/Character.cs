using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Cinemachine;

[System.Serializable]
public abstract class Character : Entity
{
    [SerializeField]
    protected float speed;
    public float Speed { get => speed; set => speed = value; }

    [SerializeField]
    protected float collisionOffset;
    public float CollisionOffset { get => collisionOffset; set => collisionOffset = value; }

    // Controller
    public ICharacterController Controller;
    public bool IsPlayerControlled = false;

    // Actions
    public List<Action> BaseActions; // Actions that are always available to the character
    public Action CurrentAction; // The action that the character is currently executing
    public Queue<Action> ActionQueue; // The queue of actions that the character will execute
    public bool IsRequestingAction;

    protected bool PauseCharacter = false;

    // Animations
    protected string CurrentAnimation;

    // References
    public Rigidbody2D rb;

    // Item animation
    public GameObject iconContainer;

    // Cameras
    public CinemachineVirtualCamera camera;


    // UI Icons
    [SerializeField]
    private SpriteRenderer speechIcon;
    public SpriteRenderer SpeechIcon { get => speechIcon; set => speechIcon = value; }

    [SerializeField]
    private SpriteRenderer itemDisplay;
    public SpriteRenderer ItemDisplay { get => itemDisplay; set => itemDisplay = value; }

    [SerializeField]
    private SpriteRenderer plusIcon;
    public SpriteRenderer PlusIcon { get => plusIcon; set => plusIcon = value; }

    [SerializeField]
    private SpriteRenderer minusIcon;
    public SpriteRenderer MinusIcon { get => minusIcon; set => minusIcon = value; }


    private Coroutine displayCoroutine;

    Vector3 iconPos;
    Vector3 iconInitialScale;
    Vector3 itemPos;
    Vector3 itemInitialScale;


    // Starlight
    HashSet<Entity> observedEntities;

    protected Character(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        SpeechIcon.enabled = false;
        ItemDisplay.enabled = false;
        PlusIcon.enabled = false;
        MinusIcon.enabled = false;

        iconPos = PlusIcon.transform.localPosition;
        iconInitialScale = PlusIcon.transform.localScale;
        itemPos = ItemDisplay.transform.localPosition;
        itemInitialScale = ItemDisplay.transform.localScale;
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ActionQueue = new Queue<Action>();
        IsRequestingAction = false;

        if (IsPlayerControlled)
        {
            Controller = new PlayerCharacterController();
        }
        else
        {
            Controller = new AgentCharacterController();
        }

        observedEntities = new HashSet<Entity>();

        iconContainer = new GameObject("Icon Container");
        iconContainer.transform.position = transform.position;
    }

    protected void Update()
    {
        iconContainer.transform.position = transform.position;

        if (PauseCharacter)
        {
            return;
        }

        Controller.ProcessInput(this);

        if (CurrentAction == null)
        {
            if (ActionQueue.Count > 0)
            {
                Debug.Log("Executing " + ActionQueue.Peek().ToString() + " from queue (" + ActionQueue.Count + " remaining)");
                ExecuteAction(ActionQueue.Dequeue());
            }
            else if (!IsRequestingAction)
            {
                Debug.Log($"Requesting action for {Id}");
                List<Action> availableActions = GetAvailableActions();

                Controller.RequestAction(this, availableActions);
            }
        }

        if (CurrentAction != null)
        {
            CurrentAction.Update(this);
        }

        // Get any items nearby
        if (this is IHasInventory && (this as IHasInventory).EntityInventory.Items.Count < (this as IHasInventory).InventoryCapacity)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.GetComponent<ItemDisplay>() != null)
                {
                    ItemDisplay itemDisplay = collider.gameObject.GetComponent<ItemDisplay>();

                    bool success = ((IHasInventory)this).EntityInventory.Add(itemDisplay.item);
                    if (success)
                    {
                        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                        {
                            string json = JsonConvert.SerializeObject(new
                            {
                                type = "Observation",
                                data = new
                                {
                                    observerId = Id.ToString(),
                                    observation = Name + " picked up " + itemDisplay.item.Name + " at X: " + rb.transform.position.x + ", Y: " + rb.transform.position.y,
                                    time = Time.time
                                }
                            }, Formatting.None);

                            WebSocketClient.Instance.websocket.SendText(json);
                        }

                        PlayAddItemAnimation(itemDisplay.item.sprite);

                        Destroy(itemDisplay.gameObject);
                    }
                    else
                    {
                        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
                        {
                            string json = JsonConvert.SerializeObject(new
                            {
                                type = "Observation",
                                data = new
                                {
                                    observerId = Id.ToString(),
                                    observation = Name + " failed to pick up " + itemDisplay.item.Name + " at X: " + rb.transform.position.x + ", Y: " + rb.transform.position.y + " because their inventory is full.",
                                    time = Time.time
                                }
                            }, Formatting.None);

                            WebSocketClient.Instance.websocket.SendText(json);
                        }

                        Debug.Log("Failed to add " + itemDisplay.item.Name + " to character");
                    }
                }
            }
        }

        // TODO: move this to the entity level, ehh i mean, does it really make sense for trees to have observations?
        // Collect observations for the character and send them to the agent
        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
        {
            observedEntities = Utilities.UpdateObservedEntities(this, observedEntities, transform, 5f);
        }
    }

    public void PlayAddItemAnimation(Sprite item)
    {
        // In PlayAddItemAnimation
        SpriteRenderer newItemDisplay = Instantiate(ItemDisplay, iconContainer.transform);
        SpriteRenderer newPlusIcon = Instantiate(PlusIcon, iconContainer.transform);

        newItemDisplay.sprite = item;
        newItemDisplay.enabled = true;
        newPlusIcon.enabled = true;

        // start new coroutine
        StartCoroutine(DisplayAndHideIcons(newItemDisplay, newPlusIcon));

    }

    public void PlayRemoveItemAnimation(Sprite item)
    {
        // In PlayRemoveItemAnimation
        SpriteRenderer newItemDisplay = Instantiate(ItemDisplay, iconContainer.transform);
        SpriteRenderer newMinusIcon = Instantiate(MinusIcon, iconContainer.transform);

        newItemDisplay.sprite = item;
        newItemDisplay.enabled = true;
        newMinusIcon.enabled = true;

        // start new coroutine
        StartCoroutine(DisplayAndHideIcons(newItemDisplay, newMinusIcon));
    }

    private IEnumerator DisplayAndHideIcons(SpriteRenderer display, SpriteRenderer icon)
    {
        // You may adjust the duration as per your needs
        float displayDuration = .85f;
        float currentTime = 0f;

        Vector3 iconInitialPos = icon.transform.position;
        Color iconInitialColor = icon.color;

        Vector3 itemInitialPos = display.transform.position;
        Color itemInitialColor = display.color;

        while (currentTime < displayDuration)
        {
            currentTime += Time.deltaTime;

            // Calculate lerp factor
            float lerpFactor = currentTime / displayDuration;

            icon.transform.position = Vector3.Lerp(iconInitialPos, iconInitialPos + new Vector3(0, 0.5f, 0), lerpFactor);
            icon.color = new Color(iconInitialColor.r, iconInitialColor.g, iconInitialColor.b, Mathf.Lerp(1f, 0f, lerpFactor));

            display.transform.position = Vector3.Lerp(itemInitialPos, itemInitialPos + new Vector3(0, 0.5f, 0), lerpFactor);
            display.color = new Color(itemInitialColor.r, itemInitialColor.g, itemInitialColor.b, Mathf.Lerp(1f, 0f, lerpFactor));

            yield return null;
        }

        // Destroy the game objects after the effect
        Destroy(display.gameObject);
        Destroy(icon.gameObject);
    }

    protected void FixedUpdate()
    {
        if (PauseCharacter)
        {
            return;
        }

        if (CurrentAction != null)
        {
            CurrentAction.FixedUpdate(this);
        }
        else
        {
            PlayAnimation("idle");
        }
    }

    public string[] GetEnvironment()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 8f);
        List<Entity> entities = new List<Entity>();
        List<ItemDisplay> items = new List<ItemDisplay>();

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.GetComponent<Entity>() != null && collider.gameObject.GetComponent<Entity>() != this)
            {
                entities.Add(collider.gameObject.GetComponent<Entity>());
            }
            else if (collider.gameObject.GetComponent<ItemDisplay>() != null)
            {
                items.Add(collider.gameObject.GetComponent<ItemDisplay>());
            }
        }

        // Create a list of strings of the entities and items that are in the environment and their position & distance from the character
        List<string> environmentStringArray = new List<string>();

        foreach (Entity entity in entities)
        {
            environmentStringArray.Add(entity.Name + " (Health: " + entity.Health + "/" + entity.MaxHealth + ") [X: " + entity.transform.position.x + ", Y: " + entity.transform.position.y + ", Distance: " + Vector2.Distance(transform.position, entity.transform.position) + "m]");
        }

        foreach (ItemDisplay itemDisplay in items)
        {
            float distance = Vector2.Distance(transform.position, itemDisplay.transform.position);

            environmentStringArray.Add(itemDisplay.item.Name + " (Item ID: " + itemDisplay.Id + ") [X: " + itemDisplay.transform.position.x + ", Y: " + itemDisplay.transform.position.y + ", Distance: " + distance + "m]");
        }

        // Sort entities and items by distance from the character
        environmentStringArray.Sort((a, b) =>
        {
            float aDistance = float.Parse(a.Split(' ')[a.Split(' ').Length - 1].Replace("m]", ""));
            float bDistance = float.Parse(b.Split(' ')[b.Split(' ').Length - 1].Replace("m]", ""));

            return aDistance.CompareTo(bDistance);
        });

        return environmentStringArray.ToArray();
    }

    public abstract List<Action> GetAvailableActions();

    public void ExecuteAction(Action action)
    {
        CurrentAction = action;

        action.Execute(this);
    }

    public void TriggerActionEffect()
    {
        if (CurrentAction is AnimationAction)
        {
            ((AnimationAction)CurrentAction).TriggerEffect(this);
        }
    }

    public void FinishAction()
    {
        CurrentAction.Cleanup(this);

        CurrentAction = null;
    }

    public void Heal(int health)
    {
        Health = Mathf.Clamp(Health + health, 0, MaxHealth);
    }

    public override void TakeDamage(int damage)
    {
        // Throw out the current action if the character is taking damage
        CurrentAction = null;

        // Pause the character's actions while they're taking damage
        PauseCharacter = true;

        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);

        if (Health <= 0)
        {
            PlayAnimation("death");
        }
        else
        {
            PlayAnimation("hurt");
        }
    }

    // Used after the character has finished taking damage
    public void ReturnToIdle()
    {
        PauseCharacter = false;

        PlayAnimation("idle");
    }

    public abstract void PlayAnimation(string animationName);

    public void Flip(float movementX)
    {
        if (movementX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movementX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}