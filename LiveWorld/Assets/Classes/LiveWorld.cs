using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

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
    public class LWSecurity : MonoBehaviour
    {
        private static string aesKey = "k3NZ09bK8E4YU3mVKKnKvwmSYcGXGWNr";
        private static string aesIV = "yZPyXIzs1SxOJnjo";
        
        public static string Encrypt(string text)
        {
            byte[] plaintextbytes = Encoding.ASCII.GetBytes(text);
            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = Encoding.ASCII.GetBytes(aesKey);
            aes.IV = Encoding.ASCII.GetBytes(aesIV);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform crypto = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encrypted = crypto.TransformFinalBlock(plaintextbytes, 0, plaintextbytes.Length);
            return Encoding.ASCII.GetString(encrypted);
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
        public class Client : NetworkBehaviour
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

            public static void Disconnect()
            {
                LWLogin.Logout();
                NetworkManager.singleton.StopClient();
                NetworkManager.singleton.StopServer();
                LWInterface.NewNotification("Disconnected.", LWInterface.Notification.LWNotificationType.Message);
            }
        }
    }
    //-------------------------------------------------------------------------------------------------------
    public class LWLogin
    {
        public enum LWLoginMode
        {
            manual_connection,
            automatic_connection
        }

        public static LWLoginMode LoginMode = LWLoginMode.automatic_connection;
        public static bool isLoggedIn = false;
        private static string Domain = "http://tethys-edu.com/501.php";
        

        //Credentials used by the login system
        public static class Credentials
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

        public static IEnumerator Login()
        {

            if (Credentials.user_email != "" && Credentials.user_password != "")
            {
                LWInterface.NewNotification("Attempting to log in..", LWInterface.Notification.LWNotificationType.Message);
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

                    if (LoginMode == LWLoginMode.automatic_connection)
                    {
                        LWInterface.NewNotification(Credentials.user_name + " logged in. Connecting...", LWInterface.Notification.LWNotificationType.Success);
                        LWNetwork.Client.InitializeClient();
                    }
                    else
                    {
                        LWInterface.NewNotification(Credentials.user_name + " logged in.", LWInterface.Notification.LWNotificationType.Success);
                    }
                    //Automatically connect after logging in if configured to do so
                }
                else
                {
                    LWInterface.NewNotification("Incorrect login.", LWInterface.Notification.LWNotificationType.Error);
                    //Do something else if the login is invalid
                }
                //If the page does not return "NLI", the login is valid
                //Otherwise, the incorrect credentials were provided

                Credentials.ClearLogin();
                //Clear the user's login credentials, regardless of their validity
            }
            else
            {
                Debug.LogError("Login credentials not set, use LWLogin.Credentials.SetLogin()");
            }
        }

        public static void Logout()
        {
            Credentials.user_email = "";
            Credentials.user_ID = "";
            Credentials.user_name = "";
            Credentials.user_password = "";
            isLoggedIn = false;
        }
    }
    //-------------------------------------------------------------------------------------------------------
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

                if(Skin)
                    GUILayout.BeginArea(new Rect(Screen.width - currentPosition.x, currentPosition.y, (Screen.width / 4) - 5, 20), Skin.box);
                else
                    GUILayout.BeginArea(new Rect(Screen.width - currentPosition.x, currentPosition.y, (Screen.width / 4) - 5, 20));

                GUILayout.Label("[<color=#" + prefixColor + ">" + prefixType + "</color>] " + Text);
                GUILayout.EndArea();

                if(currentPosition.x <= 0)
                {
                    Destroy(this.gameObject);
                }

                if(currentPosition.y > Screen.height / 2)
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
                            
                            LWLogin.Credentials.SetLogin(LWSecurity.Encrypt(email), LWSecurity.Encrypt(password));
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
        }
    }
    //-------------------------------------------------------------------------------------------------------
    public abstract class LWPlayer : MonoBehaviour
    {
        public string player_name;

        [System.Serializable]
        public class Statistics
        {
            public int health;
            public int energy;
            public float attack;
            public float defense;
            public float speed;
        }

        public Statistics Stats;
    }
    //-------------------------------------------------------------------------------------------------------
    public class LWSettings : MonoBehaviour
    {
        public static float sfx_vol;
        public static float ui_vol;
        public static float music_vol;
        public static float vo_vol;

        //Generate default settings if
        public static void SetDefaultSettings()
        {
            PlayerPrefs.SetInt("firstrun", 1);
            PlayerPrefs.SetFloat("sfx_vol", .5f);
            PlayerPrefs.SetFloat("ui_vol", .5f);
            PlayerPrefs.SetFloat("music_vol", .5f);
            PlayerPrefs.SetFloat("vo_vol", .5f);
        }

        public static void GetSettings()
        {
            if (PlayerPrefs.HasKey("firstrun"))
            {
                sfx_vol = PlayerPrefs.GetFloat("sfx_vol");
                ui_vol = PlayerPrefs.GetFloat("ui_vol");
                music_vol = PlayerPrefs.GetFloat("music_vol");
                vo_vol = PlayerPrefs.GetFloat("vo_vol");
            }
            else
            {
                print("Default settings not set.");
            }
        }

        public static void SetVolumeSettings(float sfx, float ui, float music, float vo)
        {
            PlayerPrefs.SetFloat("sfx_vol", sfx);
            PlayerPrefs.SetFloat("ui_vol", ui);
            PlayerPrefs.SetFloat("music_vol", music);
            PlayerPrefs.SetFloat("vo_vol", vo);
        }
    }
    //-------------------------------------------------------------------------------------------------------
}