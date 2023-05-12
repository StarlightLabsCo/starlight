using UnityEngine;

public class SwingSword : AnimationAction
{
    public SwingSword() : base(System.Guid.NewGuid().ToString(), "SwingSword", "Swing sword")
    {

    }


    public SwingSword(string id, string name, string description) : base(id, name, description)
    {

    }


    public override bool CanExecute(Character character)
    {
        return true;
    }

    public override void Execute(Character character)
    {
        // Play animation
        character.PlayAnimation("attack");
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
        Debug.Log("Sword damage would apply now");

        // Check if hitbox collides with enemy

        // If so deal damage

    }

    public override void Cleanup(Character character)
    {
        character.PlayAnimation("idle");
    }
}