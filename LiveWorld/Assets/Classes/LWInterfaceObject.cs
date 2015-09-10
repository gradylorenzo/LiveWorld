using UnityEngine;
using System.Collections;
using LiveWorld;

public class LWInterfaceObject : LWInterface {

    public string ipAddress;
    public string ui_email = "";
    public string ui_password = "";
    private GameObject newNotification;

    void Start()
    {
        switch (LWServer.ServerType)
        {
            case (LWServer.ServerTypes.development):
                LWServer.InitializeServer(25567, 32, "", false, !Network.HavePublicAddress());
                break;
            case (LWServer.ServerTypes.live):
                LWServer.InitializeServer(25566, 32, "", false, !Network.HavePublicAddress());
                break;
        }

        ui_skin = Resources.Load("GUI/main") as GUISkin;
    }

	void OnGUI()
    {
        GUI.skin = ui_skin;

        GUILayout.BeginArea(new Rect(5, 5, Screen.width / 3, Screen.height / 2));
        GUILayout.BeginVertical();
        if (!Network.isServer && !Network.isClient && LWServer.ServerType == LWServer.ServerTypes.not_server) {
            
            ui_email = GUILayout.TextField(ui_email);
            ui_password = GUILayout.PasswordField(ui_password, '-');
            //ipAddress = GUILayout.TextField(ipAddress);
            if (GUILayout.Button("LOGIN"))
            {
                if (ui_email == "")
                {
                    NewNotification("Email required.", Notification.NotificationType.error);
                }

                if(ui_password == "")
                {
                    NewNotification("Password required.", Notification.NotificationType.error);
                }

                if(ui_email != "" && ui_password != "")
                {
                    NewNotification("Attempting to log in...", Notification.NotificationType.message);
                    LWClient.Details.SetLoginCredentials(ui_email, ui_password);
                    StartCoroutine(LWClient.DoLogin());
                    ui_password = "";
                }
            }

            
        }
        else
        {
            if (GUILayout.Button("Disconnect"))
            {
                Network.Disconnect();
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
        HomeBar.OnGUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("TOGGLE_HOMEBAR"))
        {
            if (LWClient.Details.loggedIn)
            {
                HomeBar.Toggle();
            }
        }

        if (Input.GetButtonDown("TEST_KEY"))
        {
            print("TestKey pressed");
        }
    }

    void OnConnectedToServer()
    {
        NewNotification("Connected to server", Notification.NotificationType.success);
    }

    void OnDisconnectedFromServer()
    {
        NewNotification("Disconnected", Notification.NotificationType.message);
        LWClient.Details.ClearCredentials();
        LWInterface.HomeBar.isShowing = false;
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        NewNotification("Failed to connect: " + error, Notification.NotificationType.error);
        LWClient.Details.ClearCredentials();
    }
}
