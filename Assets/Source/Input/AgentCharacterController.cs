using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AgentCharacterController : ICharacterController
{

    public void ProcessInput(Character character)
    {
        return;
    }

    public void RequestAction(Character character, List<Action> availableActions)
    {
        // Send Web Socket message
        WebSocketClient.Instance.SendWebSocketMessage();
    }

}