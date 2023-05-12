using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AgentCharacterController : ICharacterController
{

    public void ProcessInput()
    {
        // TODO: Get surrounding environment
    }

    public Task<Queue<Action>> GetActionQueueAsync()
    {
        Queue<Action> actionQueue = new Queue<Action>();

        // Generate random MoveTO action for testing
        actionQueue.Enqueue(new MoveTo(new Vector2(Random.Range(-10, 10), Random.Range(-10, 10))));
        actionQueue.Enqueue(new SwingSword());
        actionQueue.Enqueue(new MoveTo(new Vector2(Random.Range(-10, 10), Random.Range(-10, 10))));
        actionQueue.Enqueue(new SwingPickaxe());
        actionQueue.Enqueue(new MoveTo(new Vector2(Random.Range(-10, 10), Random.Range(-10, 10))));
        actionQueue.Enqueue(new SwingHammer());


        return Task.FromResult(actionQueue);
    }

}