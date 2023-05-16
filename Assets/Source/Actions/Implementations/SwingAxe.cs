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
        // Check if hitbox collides with enemy
        Collider2D[] collisions = Utilities.DetectCollisions(character.gameObject, character.transform, new Vector2(0.16f, 0.0f), new Vector2(0.22f, .15f), LayerMask.GetMask("Default"));

        Debug.DrawRay(character.transform.position + character.transform.right * 0.16f + character.transform.up * 0.0f, character.transform.right * 0.22f, Color.red, 0.1f);

        // If so deal damage
        foreach (Collider2D collision in collisions)
        {
            if (collision.gameObject.GetComponent<Tree>() != null)
            {
                collision.gameObject.GetComponent<Tree>().TakeDamage(1);
            }
        }
    }

    public override void Cleanup(Character character)
    {
        character.PlayAnimation("idle");
    }
}