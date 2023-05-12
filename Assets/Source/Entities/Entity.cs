using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string Id { get; set; }
    public string Name { get; set; }

    public Entity(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString()
    {
        return $"{Name} ({Id})";
    }

    public override bool Equals(object obj)
    {
        if (obj is Entity)
        {
            Entity entity = (Entity)obj;
            return Id == entity.Id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
