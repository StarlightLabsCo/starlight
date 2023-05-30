using UnityEngine;

public class PickupItem : Action
{
    ItemDisplay itemDisplay;

    public PickupItem(string id, string name, string description) : base(id, name, description)
    {
    }

    public PickupItem(ItemDisplay itemDisplay) : base("pickup", "Pickup", "Pickup an item")
    {
        this.itemDisplay = itemDisplay;
    }

    public override string ToString()
    {
        return $"[PickUpItem] Pick up {itemDisplay.item.Name} (Item ID: {itemDisplay.Id}) from ground. \n-- e.g. " + "{\"type\": \"PickUpItem\", \"data\": {\"characterId\": \"A1\", \"itemId\": \"" + itemDisplay.Id + "\"}}";
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
            bool wasAdded = inventory.EntityInventory.Add(itemDisplay.item);
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