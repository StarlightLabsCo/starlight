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
        actionQueue.Enqueue(new MoveTo(new Vector2(-10f, 0.0f)));
        actionQueue.Enqueue(new SwingPickaxe());
        actionQueue.Enqueue(new SwingPickaxe());
        actionQueue.Enqueue(new SwingPickaxe());


        return Task.FromResult(actionQueue);
    }

}