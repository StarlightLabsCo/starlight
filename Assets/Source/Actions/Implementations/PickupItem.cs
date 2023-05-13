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

    public override void Cleanup(Character character)
    {
        return;
    }

    public override void Execute(Character character)
    {
        if (character is IHasInventory)
        {
            IHasInventory inventory = character as IHasInventory;
            inventory.EntityInventory.Add(item);

            GameObject.Destroy(itemDisplay.gameObject);

            // Debug
            for (int i = 0; i < inventory.EntityInventory.Items.Count; i++)
            {
                Debug.Log("Inventory item: " + inventory.EntityInventory.Items[i].Name);
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