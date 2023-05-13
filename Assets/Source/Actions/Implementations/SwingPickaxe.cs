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
        // Create hitbox in front of character on certain frame
        Debug.Log("Pickaxe damage would apply now");

        // Check if hitbox collides with enemy

        // If so deal damage

    }

    public override void Cleanup(Character character)
    {
        character.PlayAnimation("idle");
    }
}