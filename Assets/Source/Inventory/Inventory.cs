using System.Collections.Generic;

public class Inventory
{
    public int Capacity;
    public List<Item> Items;

    public Inventory(int capacity)
    {
        Capacity = capacity;
        Items = new List<Item>();
    }

    public void AddItem(Item item)
    {
        if (Items.Count < Capacity)
        {
            Items.Add(item);
        }
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
    }
}