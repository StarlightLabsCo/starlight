using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICharacterController
{
    void ProcessInput(Character character);
    void RequestAction(Character character, List<Action> availableActions);
}