using UnityEngine;
using System.Collections;
using LiveWorld;

public class Interface : LWInterface {

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
                    NewNotification("Attempting connection...", Notification.NotificationType.message);
                }
                else
                {
                    NewNotification("Address required!", Notification.NotificationType.error);
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

        HomeBar.OnGUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("TOGGLE_HOMEBAR"))
        {
            HomeBar.Toggle();
        }
    }

    void OnConnectedToServer()
    {
        NewNotification("Connected to server!", Notification.NotificationType.message);
    }

    void OnDisconnectedFromServer()
    {
        NewNotification("Disconnected!", Notification.NotificationType.message);
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        NewNotification("Failed to connect: " + error, Notification.NotificationType.error);
    }
}
