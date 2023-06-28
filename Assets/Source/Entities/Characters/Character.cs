using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

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
    protected Action CurrentAction; // The action that the character is currently executing
    public Queue<Action> ActionQueue; // The queue of actions that the character will execute
    public bool IsRequestingAction;

    protected bool PauseCharacter = false;

    // Animations
    protected string CurrentAnimation;

    // References
    public Rigidbody2D rb;

    // Starlight
    public string characterId;
    HashSet<Entity> observedEntities;

    protected Character(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = characterId;
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
    }

    protected void Update()
    {
        if (PauseCharacter)
        {
            return;
        }

        Controller.ProcessInput(this);

        if (CurrentAction == null)
        {
            Debug.Log("Current action is null");
            if (ActionQueue.Count > 0)
            {
                Debug.Log("Executing " + ActionQueue.Peek().ToString() + " from queue (" + ActionQueue.Count + " remaining)");
                ExecuteAction(ActionQueue.Dequeue());
            }
            else if (!IsRequestingAction)
            {
                Debug.Log("Requesting action");
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
            // TODO: add observation here

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.GetComponent<ItemDisplay>() != null)
                {
                    ItemDisplay itemDisplay = collider.gameObject.GetComponent<ItemDisplay>();

                    bool success = ((IHasInventory)this).EntityInventory.Add(itemDisplay.item);
                    if (success)
                    {
                        Debug.Log("Added " + itemDisplay.item.Name + " to character");
                        Destroy(itemDisplay.gameObject);
                    }
                    else
                    {
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