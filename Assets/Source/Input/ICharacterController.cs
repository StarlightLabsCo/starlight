using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICharacterController
{
    void ProcessInput();
    Task<Queue<Action>> GetActionQueueAsync();
}