using System.Collections.Generic;
using UnityEngine;

public class RemoveItemFromChest : Action
{
    private Chest chest;
    private Item item;

    public RemoveItemFromChest(Chest chest, Item item) : base("remove_item_from_chest", "Remove Item From Chest", "Remove an item from a chest")
    {
        this.chest = chest;
        this.item = item;
    }

    public override string ToString()
    {
        return $"[RemoveItemFromChest] Remove {item.Name} from chest and add to inventory.\n" + "-- e.g. { \"type\": \"RemoveItemFromChest\", \"data\": { \"characterId\": \"A1\", \"chestId\": \"" + chest.Id + "\", \"itemId\": \"" + item.Id + "\" }}";
    }

    public override void Cleanup(Character character)
    {
        return;
    }

    public override void Execute(Character character)
    {
        if (character is IHasInventory)
        {
            bool success = chest.RemoveItem(item);

            if (success)
            {
                (character as IHasInventory).EntityInventory.Add(item);
                Debug.Log("Removed " + item.Name + " from chest");
            }
            else
            {
                Debug.Log("Failed to remove " + item.Name + " from chest");
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