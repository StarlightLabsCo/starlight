using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string Id;
    public string Name;

    public int Health;
    public int MaxHealth;

    public Healthbar healthbar;

    public virtual Item itemOnDeath { get; set; } = null;
    public virtual GameObject entityOnDeath { get; set; } = null;

    public Entity(string id, string name)
    {
        Id = id;
        Name = name;
    }

    protected virtual void Awake()
    {
        Health = MaxHealth;
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

        healthbar.setHealthbar(Health, MaxHealth);

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

            // Ensure the item has a Rigidbody2D component
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Add a random force to the item
                float force = 5f; // Change this value to control the amount of force
                float angle = Random.Range(0f, 360f);
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }

        if (entityOnDeath != null)
        {
            Debug.Log("Entity on death");
            GameObject entity = Instantiate(entityOnDeath);
            entity.transform.position = transform.position;
        }

        Destroy(gameObject);
    }
}
