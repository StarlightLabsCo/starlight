using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICharacterController
{
    void ProcessInput(Character character);
    Task<Queue<Action>> GetActionQueueAsync(Character character, List<Action> availableActions);
}