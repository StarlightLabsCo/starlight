using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[System.Serializable]
public class Human : Character, IHasInventory, IHasStomach
{

    private Inventory entityInventory;
    public Inventory EntityInventory { get => entityInventory; set => entityInventory = value; }

    [SerializeField]
    private int inventoryCapacity;
    public int InventoryCapacity { get => inventoryCapacity; set => inventoryCapacity = value; }

    [SerializeField]
    private float satiety;
    public float Satiety { get => satiety; set => satiety = value; }

    [SerializeField]
    private float maxSatiety;
    public float MaxSatiety { get => maxSatiety; set => maxSatiety = value; }

    public Animator HairAnimator;

    public enum HairType
    {
        Bowl,
        Curly,
        Long,
        Mop,
        Short,
        Spikey
    }

    [SerializeField]
    public HairType hairType = HairType.Mop;

    public Animator BaseAnimator;
    public Animator ToolsAnimator;

    public Human(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        BaseActions = new List<Action>();
        BaseActions.Add(new MoveTo(Vector2.zero));

        // TODO: need to come up with a better way to start characters off with items than this
        EntityInventory = new Inventory(InventoryCapacity);

        EntityInventory.Add(new Axe());
        EntityInventory.Add(new Pickaxe());
        EntityInventory.Add(new Sword());
        EntityInventory.Add(new Berries());

        camera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    protected override void Start()
    {
        base.Start();

        WebSocketClient.Instance.characterDictionary.Add(this.Id, this);
    }

    public override List<Action> GetAvailableActions()
    {
        List<Action> actions = new List<Action>();

        // Get base actions
        foreach (Action action in BaseActions)
        {
            actions.Add(action);
        }

        Vector2 offset = new Vector2(this.transform.localScale.x, 0);
        Vector2 size = new Vector2(1.2f, 1f);

        Collider2D[] collisions = Utilities.DetectCollisions(this, offset, size, LayerMask.GetMask("Obstacles"));

        foreach (Item item in EntityInventory.AsList())
        {
            if (item is ActionItem actionItem)
            {
                if (actionItem.action is AnimationAction animationAction)
                {
                    if (collisions.Length > 0 && Energy - animationAction.EnergyCost >= 0)
                    {
                        actions.Add(actionItem.action);
                    }
                }
                else
                {
                    actions.Add(actionItem.action);
                }
            }
            actions.Add(new DropItem(item));
        }

        // Get actions from environment - short range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.GetComponent<Chest>() != null)
            {
                for (int i = 0; i < EntityInventory.AsList().Count; i++)
                {
                    actions.Add(new AddItemToChest(collider.gameObject.GetComponent<Chest>(), EntityInventory.AsList()[i]));
                }

                for (int i = 0; i < collider.gameObject.GetComponent<Chest>().EntityInventory.AsList().Count; i++)
                {
                    actions.Add(new RemoveItemFromChest(collider.gameObject.GetComponent<Chest>(), collider.gameObject.GetComponent<Chest>().EntityInventory.AsList()[i]));
                }
            } else if (collider.gameObject.GetComponent<House>() != null && !collider.gameObject.GetComponent<House>().IsOccupied())
            {
                float worldTime = WorldTime.Instance.worldTime;
                float dayDuration = WorldTime.Instance.dayDuration;
                float eveningStart = dayDuration * 0.8f;
                float earlyMorningEnd = dayDuration * 0.1f;

                float currentTimeInDay = worldTime % dayDuration;

                if (Energy < 0.2 || (currentTimeInDay >= eveningStart || currentTimeInDay <= earlyMorningEnd))
                {
                    actions.Add(new Sleep(collider.gameObject.GetComponent<House>()));
                }
            }
        }

        Collider2D[] farColliders = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (Collider2D collider in farColliders) {
            Human targetHuman = collider.gameObject.GetComponent<Human>();
            if (targetHuman != null && targetHuman != this)
            { 
                actions.Add(new StartConversation(this, targetHuman, ""));
            } 
        }

        return actions;

    }

    public override void PlayAnimation(string animationName)
    {
        if (CurrentAnimation == animationName)
        {
            return;
        }

        CurrentAnimation = animationName;

        string hairAnimationName = "human_hair_" + hairType.ToString().ToLower() + "_" + animationName;
        HairAnimator.Play(hairAnimationName, -1, 0f);
        BaseAnimator.Play("human_base_" + animationName, -1, 0f);
        ToolsAnimator.Play("human_tools_" + animationName, -1, 0f);
    }
}