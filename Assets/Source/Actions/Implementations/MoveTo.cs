using Pathfinding;
using UnityEngine;

public class MoveTo : Action
{
    // A* pathfinding
    public Vector2 target;
    protected float nextWaypointDistance = .2f;
    protected Path path;
    protected int currentWaypoint = 0;

    public Seeker seeker;

    public MoveTo(string id, string name, string description) : base(id, name, description)
    {

    }

    public MoveTo(Vector2 target) : base(System.Guid.NewGuid().ToString(), "MoveTo", "Move to a target location")
    {
        this.target = target;
    }

    public override bool CanExecute(Character character)
    {
        // TODO: Check if the target is reachable
        return true;
    }

    public override void Execute(Character character)
    {
        seeker = character.GetComponent<Seeker>();
        seeker.StartPath(character.transform.position, target, OnPathComplete);
    }


    public override void Update(Character character)
    {
        return;
    }

    public override void FixedUpdate(Character character)
    {
        if (currentWaypoint >= path.vectorPath.Count)
        {
            character.FinishAction();
            return;
        }

        // Direction to the next waypoint
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - character.rb.position).normalized;

        if (direction != Vector2.zero)
        {
            // Flip the sprite if necessary
            character.Flip(direction.x);

            // Play the walking animation
            character.PlayAnimation("walking");
        }

        // Move the character
        character.rb.MovePosition(character.rb.position + direction * character.Speed * Time.fixedDeltaTime);

        // Distance to the next waypoint
        float distance = Vector2.Distance(character.rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
        }
    }

    public override void Cleanup(Character character)
    {
        return;
    }
}