using System.Collections.Generic;
using UnityEngine;

public class AddItemToChest : Action
{
    private Chest chest;
    private Item item;

    public AddItemToChest(Chest chest, Item item) : base("add_item_to_chest", "Add Item To Chest", "Add an item to a chest")
    {
        this.chest = chest;
        this.item = item;
    }

    public override string ToString()
    {
        return $"[AddItemToChest] Add {item.Name} to chest.\n" + "-- e.g. { \"type\": \"AddItemToChest\", \"data\": { \"characterId\": \"A1\", \"chestId\": \"" + chest.Id + "\", \"itemId\": \"" + item.Id + "\" }}";
    }

    public override void Cleanup(Character character)
    {
        return;
    }

    public override void Execute(Character character)
    {
        if (character is IHasInventory)
        {
            bool success = chest.AddItem(item);

            if (success)
            {
                (character as IHasInventory).EntityInventory.Remove(item);
                Debug.Log("Added " + item.Name + " to chest");
            }
            else
            {
                Debug.Log("Failed to add " + item.Name + " to chest");
            }
        }

        character.FinishAction();
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