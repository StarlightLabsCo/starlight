using System.Collections.Generic;

public class Inventory
{
    public int Capacity;
    public Item[] Items;

    public InventoryUIManager subscribedUI;

    public Inventory(int capacity)
    {
        Capacity = capacity;
        Items = new Item[Capacity];
    }

    public bool Add(Item item)
    {
        for (int i = 0; i < Capacity; i++)
        {
            if (Items[i] == null)
            {
                Items[i] = item;

                if (subscribedUI != null) subscribedUI.Render();

                return true;
            }
        }

        return false;
    }

    public bool Remove(Item item)
    {
        int index = System.Array.IndexOf(Items, item);
        if (index >= 0)
        {
            Items[index] = null;

            if (subscribedUI != null) subscribedUI.Render();

            return true;
        }
        else
        {
            return false;
        }
    }

    public List<Item> AsList()
    {
        return new List<Item>(System.Array.FindAll(this.Items, item => item != null));
    }

}
