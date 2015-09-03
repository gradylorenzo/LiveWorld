using UnityEngine;
using System.Collections;
using LiveWorld;

public class Interface : MonoBehaviour {

    public string ipAddress;
    private GameObject newNotification;

	void OnGUI()
    {
        if (!Network.isServer && !Network.isClient) {
            ipAddress = GUI.TextField(new Rect(5, 5, 100, 20), ipAddress);
            if (GUI.Button(new Rect(5, 30, 100, 20), "Connect"))
            {
                if (ipAddress != "")
                {
                    LWClientMethod.ConnectToServer(ipAddress, 25566, "", false);
                    LWInterface.NewNotification("Attempting connection...", LWInterface.Notification.NotificationType.message);
                }
                else
                {
                    LWInterface.NewNotification("Address required.", LWInterface.Notification.NotificationType.error);
                }
            }
            if (GUI.Button(new Rect(5, 55, 100, 20), "Server")){
                LWServerMethod.InitializeServer(25566, 32, "", false, !Network.HavePublicAddress());
            }
        }
        else
        {
            if(GUI.Button(new Rect(5, 5, 100, 20), "Disconnect"))
            {
                Network.Disconnect();
            }
        }

        
    }

    void OnConnectedToServer()
    {
        LWInterface.NewNotification("Connected to server!", LWInterface.Notification.NotificationType.message);
    }

    void OnDisconnectedFromServer()
    {
        LWInterface.NewNotification("Disconnected!", LWInterface.Notification.NotificationType.message);
    }

    void OnFailedToConnect()
    {
        LWInterface.NewNotification("Failed to connect!", LWInterface.Notification.NotificationType.error);
    }
}
