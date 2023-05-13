using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCharacterController : ICharacterController
{
    public Vector2 direction;

    public void ProcessInput(Character character)
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        direction = new Vector2(moveHorizontal, moveVertical).normalized;
    }

    public Task<Queue<Action>> GetActionQueueAsync(Character character, List<Action> availableActions)
    {
        if (direction != Vector2.zero)
        {
            Queue<Action> actionQueue = new Queue<Action>();
            actionQueue.Enqueue(new Move(direction));
            return Task.FromResult(actionQueue);
        }

        return Task.FromResult(new Queue<Action>());
    }
}