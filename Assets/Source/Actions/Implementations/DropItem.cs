using UnityEngine;

public class DropItem : Action
{
    Item item;

    public DropItem(string id, string name, string description) : base(id, name, description)
    {
    }

    public DropItem(Item item) : base("drop", "Drop", "Drop an item from your inventory")
    {
        this.item = item;
    }

    public override string ToString()
    {
        return $"[DropItem] Drop {item.Name} from inventory to ground.\n-- e.g. " + "{\"type\": \"DropItem\", \"data\": {\"characterId\": \"A1\", \"itemId\": \"" + item.Id + "\"}}";
    }

    public override void Cleanup(Character character)
    {
        return;
    }

    public override void Execute(Character character)
    {
        if (character is IHasInventory)
        {
            Debug.Log("Dropping item " + item.Name + " from inventory");
            Debug.Log("Inventory before: ");
            foreach (Item i in (character as IHasInventory).EntityInventory.Items)
            {
                Debug.Log("- " + i.Name);
            }

            IHasInventory inventory = character as IHasInventory;
            bool wasRemoved = inventory.EntityInventory.Remove(item);
            if (wasRemoved)
            {
                Debug.Log("Item removed from inventory");

                GameObject itemDisplay = GameObject.Instantiate(Resources.Load<GameObject>("Item"));
                itemDisplay.GetComponent<ItemDisplay>().item = item;
                itemDisplay.transform.position = character.transform.position;

                // Apply a slight force so it moves away from the player in the direction they are facing
                Rigidbody2D rb = itemDisplay.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float force = 5f; // Change this value to control the amount of force
                    Vector2 direction = character.transform.localScale.x > 0 ? Vector2.right : Vector2.left; // Get the direction the character is facing

                    // Add a slight random bias to the direction
                    float randomBias = Random.Range(-0.2f, 0.2f); // Change these values to control the amount of bias
                    direction = new Vector2(direction.x + randomBias, direction.y + randomBias).normalized;

                    rb.AddForce(direction * force, ForceMode2D.Impulse);
                }

                // TODO: add memory observation here 
            }
            else
            {
                // TODO: add memory observation here 
                Debug.Log("Item not found in inventory");
            }

            character.FinishAction();
        }
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