using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AgentCharacterController : ICharacterController
{

    public void ProcessInput(Character character)
    {
        return;
    }

    public Task<Queue<Action>> GetActionQueueAsync(Character character, List<Action> availableActions)
    {
        Queue<Action> actionQueue = new Queue<Action>();

        // Generate random MoveTO action for testing
        actionQueue.Enqueue(availableActions[2]);
        actionQueue.Enqueue(new MoveTo(new Vector2(8.23f, 0.0f)));

        return Task.FromResult(actionQueue);
    }

}