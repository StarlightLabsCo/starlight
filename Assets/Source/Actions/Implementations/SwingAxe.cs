using UnityEngine;

public class SwingAxe : AnimationAction
{
    public SwingAxe() : base(System.Guid.NewGuid().ToString(), "SwingAxe", "Swing Axe")
    {

    }


    public SwingAxe(string id, string name, string description) : base(id, name, description)
    {

    }

    public override void Execute(Character character)
    {
        // Play animation
        character.PlayAnimation("axe");
    }

    public override void Update(Character character)
    {
        return;
    }

    public override void FixedUpdate(Character character)
    {
        return;
    }

    public override void TriggerEffect(Character character)
    {
        // Define offset and size as Vector2
        Vector2 offset = new Vector2(character.transform.localScale.x, 0);
        Vector2 size = new Vector2(1.2f, 1f);

        Collider2D[] collisions = Utilities.DetectCollisions(character, offset, size, LayerMask.GetMask("Default"));

        // If so deal damage
        foreach (Collider2D collision in collisions)
        {
            if (collision.gameObject.GetComponent<ChoppableEntity>() != null)
            {
                Debug.Log("Tree hit");
                collision.gameObject.GetComponent<ChoppableEntity>().TakeDamage(5);
            }
        }
    }

    public override void Cleanup(Character character)
    {
        character.PlayAnimation("idle");
    }
}