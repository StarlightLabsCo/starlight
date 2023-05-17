using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCharacterController : ICharacterController
{
    public Vector2 direction;
    public bool isMouseClicked;
    private float lastActionTime;
    private float actionCooldown = 0f; // Cooldown in seconds between actions

    public void ProcessInput(Character character)
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        direction = new Vector2(moveHorizontal, moveVertical).normalized;

        // Only register a mouse click if enough time has passed since the last action
        if (Input.GetMouseButtonDown(0) && Time.time - lastActionTime > actionCooldown)
        {
            isMouseClicked = true;
            lastActionTime = Time.time;
        }
        else
        {
            isMouseClicked = false;
        }
    }

    public Task<Queue<Action>> GetActionQueueAsync(Character character, List<Action> availableActions)
    {
        Queue<Action> actionQueue = new Queue<Action>();

        if (direction != Vector2.zero)
        {
            actionQueue.Enqueue(new Move(direction));
        }

        if (isMouseClicked)
        {
            actionQueue.Enqueue(new SwingPickaxe());
        }

        return Task.FromResult(actionQueue);
    }
}