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
    public class LWInterface
    {
        public class HomeBar
        {

        }


        public static void NewNotification (string text, Notification.LWNotificationType Type)
        {

        }

        //Class to inherit and attach to gameobjects as a Notification
        public class Notification
        {
            public enum LWNotificationType
            {
                success,
                message,
                warning,
                error
            }
            public string text = "Hello World";
            public LWNotificationType Type = LWNotificationType.message;

            private Vector2 currentPosition;
            private Vector2 wantedPosition;

            void Start()
            {

            }

            void OnGUI()
            {

            }
        }
    }
    //-------------------------------------------------------------------------------------------------------
}
