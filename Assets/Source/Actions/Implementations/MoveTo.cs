using NativeWebSocket;
using Newtonsoft.Json;
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


    public MoveTo(Vector2 target) : base(System.Guid.NewGuid().ToString(), "move_to", "Move to a target location", JsonConvert.SerializeObject(new
    {
        type = "object",
        properties = new
        {
            characterId = new
            {
                type = "string",
                description = "The character ID of the character that is adding the item to the chest."
            },
            x = new
            {
                type = "number",
                description = "The X coordinate of the target location."
            },
            y = new
            {
                type = "number",
                description = "The Y coordinate of the target location."
            }
        }
    }))
    {
        this.target = target;
    }

    public override string ToString()
    {
        return "[MoveTo] Move to a specific target location (X, Y). \n-- e.g. { \"type\": \"MoveTo\", \"data\": { \"characterId\": \"A1\", \"x\": -0.6865366, \"y\": -1.433641 }}";
    }

    public override void Execute(Character character)
    {
        seeker = character.GetComponent<Seeker>();
        seeker.StartPath(character.transform.position, target, OnPathComplete);

        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
        {
            string json = JsonConvert.SerializeObject(new
            {
                type = "Observation",
                data = new
                {
                    observerId = character.Id.ToString(),
                    observation = character.Name + " has started walking to " + target.x + ", Y: " + target.y + ".",
                    time = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }
    }


    public override void Update(Character character)
    {
        return;
    }

    private float collisionRadius = 0.5f;
    private int collisionRetryCount = 0;
    private int maxCollisionRetries = 5;
    private float collisionPauseTime = 1.0f;
    private float timeToUnpause;

    public override void FixedUpdate(Character character)
    {
        if (path == null) return;
        if (Time.time < timeToUnpause) return;

        if (currentWaypoint >= path.vectorPath.Count || collisionRetryCount >= maxCollisionRetries)
        {
            Debug.Log("End Of Path Reached");
            character.FinishAction();
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - character.rb.position).normalized;
        Collider2D hitCollider = Physics2D.OverlapCircle(character.rb.position + direction * character.Speed * Time.fixedDeltaTime, collisionRadius);

        if (hitCollider != null && hitCollider.gameObject != character.gameObject)
        {
            timeToUnpause = Time.time + collisionPauseTime;
            collisionRetryCount++;
            return;
        }

        collisionRetryCount = 0;

        if (direction != Vector2.zero)
        {
            character.Flip(direction.x);
            character.PlayAnimation("walking");
        }

        character.rb.MovePosition(character.rb.position + direction * character.Speed * Time.fixedDeltaTime);
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
        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
        {
            string json = JsonConvert.SerializeObject(new
            {
                type = "ActionExecuted",
                data = new
                {
                    characterId = character.Id.ToString(),
                    result = character.Name + " walked to X: " + target.x + ", Y: " + target.y + ".",
                    resultTime = Time.time
                }
            }, Formatting.None);

            WebSocketClient.Instance.websocket.SendText(json);
        }
    }
}