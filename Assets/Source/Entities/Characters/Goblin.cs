using System.Collections.Generic;
using UnityEngine;

public class Goblin : Character, IHasInventory
{

    private Inventory entityInventory;
    public Inventory EntityInventory { get => entityInventory; set => entityInventory = value; }

    [SerializeField]
    private int inventoryCapacity;
    public int InventoryCapacity { get => inventoryCapacity; set => inventoryCapacity = value; }

    public Animator Animator;

    public Goblin(string id, string name) : base(id, name)
    {
    }

    protected override void Start()
    {
        base.Start();

        EntityInventory = new Inventory(InventoryCapacity);
    }

    public override List<Action> GetAvailableActions()
    {
        List<Action> actions = new List<Action>();

        // Get base actions
        foreach (Action action in BaseActions)
        {
            if (action.CanExecute(this))
            {
                actions.Add(action);
            }
        }

        // Get available actions from inventory
        foreach (Item item in EntityInventory.Items)
        {
            if (item is IUseable && ((IUseable)item).Action.CanExecute(this))
            {
                actions.Add(((IUseable)item).Action);
            }
        }

        return actions;
    }
    public override void PlayAnimation(string animationName)
    {
        base.PlayAnimation(animationName);

        Animator.Play("goblin_" + animationName);
    }
}