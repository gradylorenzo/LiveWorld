using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace LiveWorld
{
    public class LWInterface : MonoBehaviour
    {
        //Static class to call when creating a new notification.
        public static void NewNotification(string text, Notification.LWNotificationType Type)
        {
            GameObject newNotification;
            newNotification = Resources.Load("GUI/LWNotificationObject") as GameObject;
            newNotification.GetComponent<Notification>().Text = text;
            newNotification.GetComponent<Notification>().Type = Type;
            Instantiate(newNotification, Vector3.zero, new Quaternion(0, 0, 0, 0));
            newNotification = null;
        }

        //Class to inherit to attach to player gameobjects. Keep the component inactive on
        //non-local players, only activate if local player
        public abstract class HomeBar : MonoBehaviour
        {
            [System.Serializable]
            public class Showing
            {
                public bool main = false;
                public bool player = false;
                public bool social = false;
                public bool world = false;
                public bool settings = false;
            }

            public Showing isShowing;
            public GUISkin Skin;
            public float slideSpeed = .2f;
            private float wantedY;
            private float currentY;

            void Start()
            {
                currentY = 0;
                wantedY = 0;
            }

            void OnGUI()
            {
                if (Skin)
                {
                    GUI.skin = Skin;
                    Skin.button.fixedWidth = Screen.width / 4;
                }

                currentY = Mathf.Lerp(currentY, wantedY, slideSpeed);

                if (isShowing.main)
                    wantedY = 20;
                else
                    wantedY = 0;

                GUILayout.BeginArea(new Rect(0, Screen.height - currentY, Screen.width, 20));
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Player"))//use LWLogin.Credentials.user_name
                {
                    NewNotification("Player clicked!", Notification.LWNotificationType.Success);
                    isShowing.player = true;
                    isShowing.social = false;
                    isShowing.world = false;
                }
                if (GUILayout.Button("Social"))
                {
                    NewNotification("Social clicked!", Notification.LWNotificationType.Message);
                    isShowing.player = false;
                    isShowing.social = true;
                    isShowing.world = false;
                }
                if (GUILayout.Button("World"))
                {
                    NewNotification("World clicked!", Notification.LWNotificationType.Warning);
                    isShowing.player = false;
                    isShowing.social = false;
                    isShowing.world = true;
                }
                if (GUILayout.Button("Settings"))
                {
                    NewNotification("Settings clicked!", Notification.LWNotificationType.Error);
                    //Allow settings to be displayed even if other windows are displayed
                    isShowing.settings = !isShowing.settings;
                    LWSettings.SetDefaultSettings();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            void Update()
            {
                if (Input.GetButtonDown("TOGGLE_HOMEBAR"))
                {
                    isShowing.main = !isShowing.main;
                }

                if (isShowing.main)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }

                Cursor.visible = isShowing.main;
            }
        }

        //Class to inherit and attach to gameobjects as a Notification
        public abstract class Notification : MonoBehaviour
        {
            public enum LWNotificationType
            {
                Success,
                Message,
                Warning,
                Error
            }
            public string Text = "Hello World";
            public LWNotificationType Type = LWNotificationType.Message;
            public float slideSpeed = .05f;
            public GUISkin Skin;

            private Vector2 wantedPosition;
            private Vector2 currentPosition;

            private string prefixType;
            private string prefixColor;

            void Start()
            {
                DontDestroyOnLoad(this.gameObject);
                prefixType = Type.ToString();
                switch (Type)
                {
                    case (LWNotificationType.Success):
                        prefixColor = "00ff00ff";
                        break;
                    case (LWNotificationType.Message):
                        prefixColor = "00ffffff";
                        break;
                    case (LWNotificationType.Warning):
                        prefixColor = "ffff00ff";
                        break;
                    case (LWNotificationType.Error):
                        prefixColor = "ff0000ff";
                        break;
                }
                currentPosition = new Vector2(Screen.width / 4, -50);
                wantedPosition = new Vector2(Screen.width / 4, 5);
                this.gameObject.tag = "LWNotificationObject";

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("LWNotificationObject"))
                {
                    if (go != this.gameObject)
                    {
                        go.GetComponent<Notification>().wantedPosition.y += 25;
                    }
                }

                Invoke("Destroy", 5);
            }

            void OnGUI()
            {
                if (Skin && GUI.skin != null)
                {
                    GUI.skin = Skin;
                }

                currentPosition = Vector2.Lerp(currentPosition, wantedPosition, slideSpeed);

                if (Skin)
                    GUILayout.BeginArea(new Rect(Screen.width - currentPosition.x, currentPosition.y, (Screen.width / 4) - 5, 20), Skin.box);
                else
                    GUILayout.BeginArea(new Rect(Screen.width - currentPosition.x, currentPosition.y, (Screen.width / 4) - 5, 20));

                GUILayout.Label("[<color=#" + prefixColor + ">" + prefixType + "</color>] " + Text);
                GUILayout.EndArea();

                if (currentPosition.x <= 0)
                {
                    Destroy(this.gameObject);
                }

                if (currentPosition.y > Screen.height / 2)
                {
                    Destroy();
                }
            }

            void Destroy()
            {
                wantedPosition.x = -1;
            }
        }

        public abstract class Connection : NetworkManager
        {
            public GUISkin skin;
            public string email;
            public string password;

            void Start()
            {
                if (LWNetwork.NetworkMode == LWNetwork.LWNetworkModes.dev_server || LWNetwork.NetworkMode == LWNetwork.LWNetworkModes.rel_server)
                {
                    LWNetwork.Server.InitializeServer();
                }
            }

            void OnGUI()
            {

                GUI.skin = skin;

                GUILayout.BeginArea(new Rect(5, 5, Screen.width / 4, Screen.height / 4));

                GUILayout.BeginVertical();
                if (!LWLogin.isLoggedIn && (LWNetwork.NetworkMode == LWNetwork.LWNetworkModes.dev_client || LWNetwork.NetworkMode == LWNetwork.LWNetworkModes.rel_client))
                {
                    email = GUILayout.TextField(email);
                    password = GUILayout.PasswordField(password, '*');
                    if (GUILayout.Button("CONNECT"))
                    {
                        if (email != "" && password != "")
                        {

                            LWLogin.Credentials.SetLogin(email, password);
                            StartCoroutine(LWLogin.Login());
                            password = string.Empty;
                        }
                        else
                        {
                            if (email == "")
                            {
                                NewNotification("Email required.", Notification.LWNotificationType.Error);
                            }

                            if (password == "")
                            {
                                NewNotification("Password required.", Notification.LWNotificationType.Error);
                            }
                        }
                    }
                }
                else
                {

                    if (GUILayout.Button("DISCONNECT") && (LWNetwork.NetworkMode == LWNetwork.LWNetworkModes.dev_client || LWNetwork.NetworkMode == LWNetwork.LWNetworkModes.rel_client))
                    {

                        LWNetwork.Client.Disconnect();
                    }
                }
                GUILayout.EndVertical();

                GUILayout.EndArea();
            }

            void OnClientConnect()
            {
                LWInterface.NewNotification("Connected!", Notification.LWNotificationType.Success);
            }
        }
    }
}