using UnityEngine;

namespace WebSocketEvents
{
    public class WebSocketEvent
    {
        public string type { get; set; }
    }

    public class SwingAxeEvent : WebSocketEvent
    {
        public string characterId { get; set; }
    }

    public class SwingPickaxeEvent : WebSocketEvent
    {
        public string characterId { get; set; }
    }

    public class SwingSwordEvent : WebSocketEvent
    {
        public string characterId { get; set; }
    }

    public class MoveToEvent : WebSocketEvent
    {
        public string characterId { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }

    public class MoveEvent : WebSocketEvent
    {
        public string characterId { get; set; }
        public string location { get; set; }
    }
}