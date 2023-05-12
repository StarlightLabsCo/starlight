using UnityEngine;

public abstract class Item
{
    string ID { get; set; }
    string Name { get; set; }

    Sprite Sprite { get; set; }

    public Item(string id, string name)
    {
        ID = id;
        Name = name;
    }

    public override string ToString()
    {
        return $"{Name} ({ID})";
    }

    public override bool Equals(object obj)
    {
        if (obj is Item)
        {
            Item item = (Item)obj;
            return ID == item.ID;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}