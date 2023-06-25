using UnityEngine;


public abstract class Item
{
    public string Id;
    public string Name;

    public Sprite sprite;

    public override bool Equals(object obj)
    {
        return obj is Item item &&
               Id == item.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}