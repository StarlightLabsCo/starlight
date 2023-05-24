using System.Collections.Generic;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

public class AgentCharacterController : ICharacterController
{

    public void ProcessInput(Character character)
    {
        return;
    }

    public void RequestAction(Character character, List<Action> availableActions)
    {
        if (WebSocketClient.Instance.websocket.State == WebSocketState.Open)
        {

            // Send Web Socket message
            WebSocketClient.Instance.SendWebSocketMessage(character);

            character.IsRequestingAction = true;
        }
    }

}