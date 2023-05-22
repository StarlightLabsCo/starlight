using System.Collections;
using System.Collections.Generic;
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
    public Animator BaseAnimator;
    public Animator ToolsAnimator;

    public Human(string id, string name) : base("human", "Human")
    {
    }

    protected override void Start()
    {
        base.Start();

        BaseActions = new List<Action>();
        BaseActions.Add(new Move());
        BaseActions.Add(new MoveTo());

        EntityInventory = new Inventory(InventoryCapacity);
        EntityInventory.Add(new Axe());
    }

    public override List<Action> GetAvailableActions()
    {
        List<Action> actions = new List<Action>();

        // Get base actions
        foreach (Action action in BaseActions)
        {
            actions.Add(action);
        }

        // Get available actions from inventory
        foreach (Item item in EntityInventory.Items)
        {
            if (item is ActionItem)
            {
                actions.Add((item as ActionItem).action);
            }
        }

        // Get actions from environment
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.GetComponent<ItemDisplay>() != null)
            {
                actions.Add(new PickupItem(collider.gameObject.GetComponent<ItemDisplay>()));
            }
            else if (collider.gameObject.GetComponent<Chest>() != null)
            {
                if (EntityInventory.Items.Count > 0)
                {
                    actions.Add(new AddItemToChest(collider.gameObject.GetComponent<Chest>(), EntityInventory.Items[0]));
                }
            }
        }

        return actions;

    }

    public override void PlayAnimation(string animationName)
    {
        base.PlayAnimation(animationName);

        HairAnimator.Play("human_hair_" + animationName);
        BaseAnimator.Play("human_base_" + animationName);
        ToolsAnimator.Play("human_tools_" + animationName);
    }
}