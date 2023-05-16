using UnityEngine;

public class SwingPickaxe : AnimationAction
{
    public SwingPickaxe() : base(System.Guid.NewGuid().ToString(), "SwingPickaxe", "Swing Pickaxe")
    {

    }


    public SwingPickaxe(string id, string name, string description) : base(id, name, description)
    {

    }

    public override void Execute(Character character)
    {
        // Play animation
        character.PlayAnimation("mining");
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
        Debug.Log("Trigger Effect");

        Collider2D[] collisions = Utilities.DetectCollisions(character.gameObject, character.transform, new Vector2(0.16f, 0.0f), new Vector2(0.22f, .15f), LayerMask.GetMask("Obstacles"));

        Debug.DrawRay(character.transform.position + character.transform.right * 0.16f + character.transform.up * 0.0f, character.transform.right * 0.22f, Color.red, 0.1f);

        // If so deal damage
        foreach (Collider2D collision in collisions)
        {
            Debug.Log("Collision: " + collision.gameObject.name);
            if (collision.gameObject.GetComponent<CopperOre>() != null)
            {
                Debug.Log("Hit copper ore");
                collision.gameObject.GetComponent<CopperOre>().TakeDamage(5);
            }
        }

    }

    public override void Cleanup(Character character)
    {
        Debug.Log("SwingPickaxe cleanup");
        character.PlayAnimation("idle");
    }
}