using System.Collections.Generic;
using UnityEngine;

public class OpenChest : Action
{
    private Chest chest;

    public OpenChest(Chest chest) : base($"open_chest_{chest.Id}", $"open_chest_{chest.Id}", $"Open chest ${chest.Id} to see what's inside.", "")
    {
        this.chest = chest;
    }

    public override string ToString()
    {
        return $"[OpenChest] Open chest & view contents.";
    }

    public override void Cleanup(Character character)
    {
        return;
    }

    public override void Execute(Character character)
    {
        if (character.IsPlayerControlled)
        {
            // just read the items of the chest
            List<Item> items = chest.ViewItems();
            foreach (Item item in items)
            {
                Debug.Log(item.Name);
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