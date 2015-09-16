using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace LiveWorld
{
    public class LWTest : MonoBehaviour
    {
        public static void Test()
        {
            print("Hello World");
        }
    }
    //-------------------------------------------------------------------------------------------------------
    public class LWNetwork : NetworkBehaviour
    {
        public enum LWNetworkModes
        {
            rel_server,//Release server mode
            dev_server,//Development server Mode
            rel_client,//Release client mode
            dev_client,//Development client mode
            rel_host,//Release host mode
            dev_host//Development host mode
        }

        public static LWNetworkModes NetworkMode = LWNetworkModes.dev_client;

        //Method class for server-side behavior
        public class Server
        { 
            public static void InitializeServer()
            {
                if(NetworkMode == LWNetworkModes.dev_server)
                {
                    NetworkManager.singleton.networkPort = 7778;
                    NetworkManager.singleton.StartServer();
                }
                else if(NetworkMode == LWNetworkModes.rel_server)
                {
                    NetworkManager.singleton.networkPort = 7777;
                    NetworkManager.singleton.StartServer();
                }
                else
                {
                    Debug.LogError("Not in any server mode, adjust LWNetwork.NetworkMode");
                }
                //Start server on 7778 if using development server
                //Start server on 7777 if using release server
                //Otherwise, log an error for no server mode
            }
        }

        //Method class for client-side behavior
        public class Client
        {
            public static void InitializeClient()
            {
                if (NetworkMode == LWNetworkModes.dev_client)
                {
                    NetworkManager.singleton.networkPort = 7778;
                    NetworkManager.singleton.networkAddress = "127.0.0.1";
                    NetworkManager.singleton.StartClient();
                }
                else if (NetworkMode == LWNetworkModes.rel_client)
                {
                    NetworkManager.singleton.networkPort = 7777;
                    NetworkManager.singleton.networkAddress = "127.0.0.1";
                    NetworkManager.singleton.StartClient();
                }
                else
                {
                    Debug.LogError("Not in any client mode, adjust LWNetwork.NetworkMode");
                }
                //Start client on 7778 if using development client
                //Start client on 7777 if using release client
                //Otherwise, log an error for no client mode
            }
        }
    }
    //-------------------------------------------------------------------------------------------------------
    public class LWLogin
    {
        //Credentials used by the login system
        public class Credentials
        {
            public static string user_ID;
            public static string user_name;
            public static string user_email;
            public static string user_password;

            public static void ClearLogin()
            {
                user_email = "";
                user_password = "";
            }

            public static void ClearUser()
            {
                user_ID = "";
                user_name = "";
            }

            public static void SetLogin(string email, string password)
            {
                user_email = email;
                user_password = password;
            }

            public static void SetUser(string id, string name)
            {
                user_ID = id;
                user_name = name;
            }
        }

        public enum LWLoginMode
        {
            manual_connection,
            automatic_connection
        }

        public static LWLoginMode LoginMode = LWLoginMode.automatic_connection;
        public static bool isLoggedIn = false;
        private static string Domain = "http://tethys-edu.com/501.php";

        public static IEnumerator Login()
        {
            if (Credentials.user_email != "" && Credentials.user_password != "")
            {
                WWWForm requestForm = new WWWForm();
                requestForm.AddField("email", Credentials.user_email);
                requestForm.AddField("password", Credentials.user_password);
                WWW request = new WWW(Domain, requestForm);

                yield return request;

                string requestText = request.text;

                if (requestText != "NLI")
                {
                    string[] creds;
                    char delim = '&';
                    creds = requestText.Split(delim);

                    Credentials.SetUser(creds[0], creds[1]);
                    isLoggedIn = true;
                }
                else
                {
                    //Do something else if the login is invalid
                }
                //If the page does not return "NLI", the login is valid
                //Otherwise, the incorrect credentials were provided

                Credentials.ClearLogin();
                //Clear the user's login credentials, regardless of their validity

                if(LoginMode == LWLoginMode.automatic_connection)
                {
                    LWNetwork.Client.InitializeClient();
                }
                //Automatically connect after logging in if configured to do so
            }
            else
            {
                Debug.LogError("Login credentials not set, use LWLogin.Credentials.SetLogin()");
            }
        }
    }
    //-------------------------------------------------------------------------------------------------------
    public class LWInterface : MonoBehaviour
    {
        //Class to inherit to attach to player gameobjects. Keep the component inactive on
        //non-local players, only activate if local player
        public class HomeBar
        {
            
        }

        //Static class to call when creating a new notification.
        public static void NewNotification (string text, Notification.LWNotificationType Type)
        {
            GameObject newNotification;
            newNotification = Resources.Load("GUI/LWNotificationObject") as GameObject;
            newNotification.GetComponent<Notification>().Text = text;
            newNotification.GetComponent<Notification>().Type = Type;
            Instantiate(newNotification, Vector3.zero, new Quaternion(0, 0, 0, 0));
            newNotification = null;
        }

        //Class to inherit and attach to gameobjects as a Notification
        public class Notification : MonoBehaviour
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

                foreach(GameObject go in GameObject.FindGameObjectsWithTag("LWNotificationObject"))
                {
                    if(go != this.gameObject)
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

                GUILayout.BeginArea(new Rect(Screen.width - currentPosition.x, currentPosition.y, (Screen.width / 4) - 5, 20), Skin.box);
                GUILayout.Label("[<color=#" + prefixColor + ">" + prefixType + "</color>] " + Text);
                GUILayout.EndArea();

                if(currentPosition.x <= 0)
                {
                    Destroy(this.gameObject);
                }
            }

            void Destroy()
            {
                wantedPosition.x = -1;
            }
        }
    }
    //-------------------------------------------------------------------------------------------------------
    public class LWPlayer
    {

    }
    //-------------------------------------------------------------------------------------------------------
}
