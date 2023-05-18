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

    public bool Add(Item item)
    {
        if (Items.Count < Capacity)
        {
            Items.Add(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Remove(Item item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            return true;
        }
        else
        {
            return false;
        }
    }
}