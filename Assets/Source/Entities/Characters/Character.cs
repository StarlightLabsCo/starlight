using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public abstract class Character : Entity, IDamagable, IHealable
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
        Controller.ProcessInput();

        if (CurrentAction == null)
        {
            if (ActionQueue.Count > 0)
            {
                ExecuteAction(ActionQueue.Dequeue());
            }
            else if (!IsRequestingAction)
            {
                StartCoroutine(GetActionQueueCoroutine());
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

    private IEnumerator GetActionQueueCoroutine()
    {
        IsRequestingAction = true;

        Task<Queue<Action>> getQueueTask = Controller.GetActionQueueAsync();

        // Wait for the server request to complete or for a timeout
        yield return new WaitUntil(() => getQueueTask.IsCompleted || getQueueTask.IsFaulted);

        IsRequestingAction = false;

        if (getQueueTask.IsFaulted)
        {
            // Handle error...
        }
        else
        {
            for (int i = 0; i < getQueueTask.Result.Count; i++)
            {
                Debug.Log($"Got action {getQueueTask.Result.Peek().Name} from server");
                ActionQueue.Enqueue(getQueueTask.Result.Dequeue());
            }
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

    public void TakeDamage(int damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);

        if (Health == 0)
        {
            PlayAnimation("death");
        }
        else
        {
            PlayAnimation("hurt");
        }
    }

    public void Die()
    {
        Destroy(gameObject);
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