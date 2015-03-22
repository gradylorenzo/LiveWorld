using UnityEngine;
using System.Collections;
using LiveWorld;

public class ChatComponent : Chat
{
    void Start()
    {
        string message = "liveworld lead developer";
        Communication.SendChatMessage(message, this.GetComponent<PlayerComponent>().name);
    }
}
