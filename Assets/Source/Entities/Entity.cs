using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string Id { get; set; }
    public string Name { get; set; }

    public int Health;
    public int MaxHealth;

    public virtual Item itemOnDeath { get; set; } = null;

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

    public virtual void TakeDamage(int damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);

        if (Health <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        if (itemOnDeath != null)
        {
            GameObject item = Instantiate(Resources.Load("Item") as GameObject);
            item.GetComponent<ItemDisplay>().item = itemOnDeath;
            item.transform.position = transform.position;
        }

        Destroy(gameObject);
    }
}
