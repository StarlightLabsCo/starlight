using UnityEngine;

public class PickupItem : Action
{
    ItemDisplay itemDisplay;
    Item item;

    public PickupItem(string id, string name, string description) : base(id, name, description)
    {
    }

    public PickupItem(ItemDisplay itemDisplay) : base("pickup", "Pickup", "Pickup an item")
    {
        this.itemDisplay = itemDisplay;
        this.item = itemDisplay.item;
    }

    public override string ToString()
    {
        return $"[PickupItem] Pickup {item.Name} (X: {itemDisplay.gameObject.transform.position.x}, Y: {itemDisplay.gameObject.transform.position.y}) from ground.";
    }

    public override void Cleanup(Character character)
    {
        return;
    }

    public override void Execute(Character character)
    {
        if (character is IHasInventory)
        {
            IHasInventory inventory = character as IHasInventory;
            bool wasAdded = inventory.EntityInventory.Add(item);
            if (wasAdded)
            {
                GameObject.Destroy(itemDisplay.gameObject);
            }
            else
            {
                Debug.Log("Inventory is full");
            }

            character.FinishAction();
        }
    }

    public override void FixedUpdate(Character character)
    {
        return;
    }

    public override void Update(Character character)
    {
        return;
    }
}