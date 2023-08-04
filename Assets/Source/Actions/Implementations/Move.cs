using System.Collections.Generic;
using UnityEngine;



public class Move : Action
{
    private Vector2 direction;

    ContactFilter2D movementFilter = new ContactFilter2D();
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();


    public Move(Vector2 direction) : base(System.Guid.NewGuid().ToString(), "Move", "Move in a direction", "")
    {
        this.direction = direction;
    }

    public override string ToString()
    {
        return $"[Move] Move based on a provided normalized vector (X, Y)." + "\n-- e.g. { \"type\": \"Move\", \"data\": { \"characterId\": \"A1\", \"x\": -1.0, \"y\": 0.0 }}";
    }

    bool isAccessible(Character character, Vector2 direction)
    {
        // Cast a ray in the direction of movement
        int numCollisions = character.rb.Cast(direction, movementFilter, castCollisions, character.CollisionOffset);

        return numCollisions == 0;
    }


    public override void Execute(Character character)
    {
        return;
    }

    public override void Update(Character character)
    {
        return;
    }

    public override void FixedUpdate(Character character)
    {
        bool moved = false;

        // Full vector
        if (isAccessible(character, direction))
        {
            character.PlayAnimation("walking");
            character.rb.MovePosition(character.rb.position + direction * character.Speed * Time.fixedDeltaTime);

            moved = true;
        }
        // Horizontal
        else if (direction.x != 0 && isAccessible(character, new Vector2(direction.x, 0)))
        {
            character.PlayAnimation("walking");
            character.rb.MovePosition(character.rb.position + new Vector2(direction.x, 0) * character.Speed * Time.fixedDeltaTime);

            moved = true;
        }
        // Vertical
        else if (direction.y != 0 && isAccessible(character, new Vector2(0, direction.y)))
        {
            character.PlayAnimation("walking");
            character.rb.MovePosition(character.rb.position + new Vector2(0, direction.y) * character.Speed * Time.fixedDeltaTime);

            moved = true;
        }
        // No movement
        else
        {
            character.PlayAnimation("idle");
        }


        // Flip character game object if necessary
        if (moved)
        {
            character.Flip(direction.x);
        }

        character.FinishAction();
    }

    public override void Cleanup(Character character)
    {
        return;
    }
}