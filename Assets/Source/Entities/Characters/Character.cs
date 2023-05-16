using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public abstract class Character : Entity
{
    // Stats
    [SerializeField]
    protected int health;
    public int Health { get => health; set => health = value; }

    [SerializeField]
    protected int maxHealth;
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }

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
    protected Queue<Action> ActionQueue; // The queue of actions that the character will execute
    protected bool IsRequestingAction;

    // Animations
    protected string CurrentAnimation;

    // References
    public Rigidbody2D rb;

    protected Character(string id, string name) : base(id, name)
    {
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
    }

    protected void Update()
    {
        Controller.ProcessInput(this);

        if (CurrentAction == null)
        {
            Debug.Log("Action queue count: " + ActionQueue.Count);
            if (ActionQueue.Count > 0)
            {
                ExecuteAction(ActionQueue.Dequeue());
            }
            else if (!IsRequestingAction)
            {
                List<Action> availableActions = GetAvailableActions();

                // Debug
                Debug.Log("Available actions:");
                foreach (Action action in availableActions)
                {
                    Debug.Log(action);
                }

                StartCoroutine(GetActionQueueCoroutine(availableActions));
            }
        }

        if (CurrentAction != null)
        {
            CurrentAction.Update(this);
        }
    }

    protected void FixedUpdate()
    {
        if (CurrentAction != null)
        {
            CurrentAction.FixedUpdate(this);
        }
        else
        {
            PlayAnimation("idle");
        }
    }

    public abstract List<Action> GetAvailableActions();

    private IEnumerator GetActionQueueCoroutine(List<Action> availableActions)
    {
        IsRequestingAction = true;

        Task<Queue<Action>> getQueueTask = Controller.GetActionQueueAsync(this, availableActions);

        // Wait for the server request to complete or for a timeout
        yield return new WaitUntil(() => getQueueTask.IsCompleted || getQueueTask.IsFaulted);

        IsRequestingAction = false;

        if (getQueueTask.IsFaulted)
        {
            // Handle error...
        }
        else
        {
            // TODO: right now we're only getting new actions once we've finished the current queue
            // TODO: but in the future, we'll want to be able to add more actions to the queue while it's running
            // TODO: or even cancel the current queue and start a new one
            ActionQueue = getQueueTask.Result;
        }
    }
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

    public virtual void PlayAnimation(string animationName)
    {
        if (CurrentAnimation == animationName)
        {
            return;
        }

        CurrentAnimation = animationName;
    }

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