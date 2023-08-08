using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[System.Serializable]
public class Human : Character, IHasInventory
{

    private Inventory entityInventory;
    public Inventory EntityInventory { get => entityInventory; set => entityInventory = value; }

    [SerializeField]
    private int inventoryCapacity;
    public int InventoryCapacity { get => inventoryCapacity; set => inventoryCapacity = value; }


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

    public Human(string id, string name) : base("human", "Human")
    {
    }

    protected override void Awake()
    {
        base.Awake();

        // Id = "human_" + System.Guid.NewGuid().ToString();
        if (Id == "A1")
        {
            Name = "Thomas Smith";
        }
        else if (Id == "A2")
        {
            Name = "George Brown";
        }
        else if (Id == "A3")
        {
            Name = "Will Turner";
        }
        else if (Id == "A4")
        {
            Name = "Lucy Wilde";
        } else if (Id == "A5")
        {
            Name = "Eli Green";
        }
    }

    protected override void Start()
    {
        base.Start();

        BaseActions = new List<Action>();
        BaseActions.Add(new MoveTo(Vector2.zero));

        EntityInventory = new Inventory(InventoryCapacity);
        EntityInventory.Add(new Axe());
        EntityInventory.Add(new Pickaxe());
        EntityInventory.Add(new Sword());

        camera = GetComponentInChildren<CinemachineVirtualCamera>();
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
        Debug.Log($"Hitbox collisions: {collisions.Length}");

        // Get available actions from inventory
        foreach (Item item in EntityInventory.Items)
        {
            if (item is ActionItem)
            {
                if ((item as ActionItem).action is AnimationAction && collisions.Length > 0)
                {
                    actions.Add((item as ActionItem).action);
                }
                else if (!((item as ActionItem).action is AnimationAction))
                {
                    actions.Add((item as ActionItem).action);
                }
            }

            actions.Add(new DropItem(item));
        }

        // Get actions from environment
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.GetComponent<Chest>() != null)
            {
                for (int i = 0; i < EntityInventory.Items.Count; i++)
                {
                    actions.Add(new AddItemToChest(collider.gameObject.GetComponent<Chest>(), EntityInventory.Items[i]));
                }

                for (int i = 0; i < collider.gameObject.GetComponent<Chest>().EntityInventory.Items.Count; i++)
                {
                    actions.Add(new RemoveItemFromChest(collider.gameObject.GetComponent<Chest>(), collider.gameObject.GetComponent<Chest>().EntityInventory.Items[i]));
                }
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