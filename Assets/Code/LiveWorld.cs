using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace LiveWorld
{
    public class Test : MonoBehaviour
    //Test method to make sure you are referencing the LiveWorld namespace correctly
    {
        public static void test(string s)
        {
            print(s);
        }
    }

    public class Chat : MonoBehaviour
    {
        public List<string> chatMessages;
    }

    public class Player : MonoBehaviour
    {
        public string id;
        public string name;
    }

    public class Connection : MonoBehaviour
    //Holds all connection-related methods, including connection, disconnection, server initialization,
    {
        public static bool isClient = Network.isClient;
        public static bool isServer = Network.isServer;
        //isClient and isServer are redundant copies of MonoBehavior's default Network.isClient and
        //Network.isServer booleans. Using either would be appropriate.

        //ipAddress - the public address of the server
        //portNumber - the port to listen from
        //connectionLimit - the maximum number of clients allowed to connect to the server
        //serverToken - the unique string used as a password by the server. This prevents alternate LiveWorld clients from connecting to your server.
        //Note that serverToken is stored client-side, so prevention is not foolproof.
        public static string ipAddress = "127.0.0.1";
        public static int portNumber = 25566;
        public static int connectionLimit = 32;
        public static string serverToken = "JNgz28Eye11mOp0LVC4FcYmtmDlLclim2WHFD5vFGxKPH3jSEah5DcoTvF4AYD08";

        public static void connectToServer()
        {
            Network.Connect(ipAddress, portNumber, serverToken);
            Debug.Log("Attempting to connect to server");
        }

        public static void initializeServer()
        {
            bool useNat = !Network.HavePublicAddress();
            Network.incomingPassword = serverToken;
            Network.InitializeServer(connectionLimit, portNumber, useNat);
            Debug.Log("Initializing server");
        }

        public static void disconnectFromServer()
        {
            if (Network.isClient || Network.isServer)
            {
                Network.Disconnect();
                Debug.Log("Disconnecting from server.");
            }
            else
            {
                Debug.LogWarning("Not currently connected to or running as a server, but script is trying to disconnect.");
            }
            print("disconnectedFromServer");
        }

        public static void deinitializeServer()
        {
            Network.Disconnect();
        }

        public void OnFailedToConnect(NetworkConnectionError cError)
        {

            if (cError == NetworkConnectionError.ConnectionFailed)
            //General connectivity problems
            {
                Debug.LogWarning("General connectivity failure.");
            }
            else if(cError == NetworkConnectionError.InvalidPassword)
            //Invalid server token
            {
                Debug.LogWarning("Invalid server token.");
            }
            else if(cError == NetworkConnectionError.TooManyConnectedPlayers)
            //Server connection limit reached
            {
                Debug.LogWarning("Server connection limit reached.");
            }
            else if(cError == NetworkConnectionError.ConnectionBanned)
            //Connections from this address are banned
            {
                Debug.LogWarning("Address banned");
            }
            else if (cError == NetworkConnectionError.AlreadyConnectedToServer)
            //There is already an active connection from this client
            {
                Debug.LogWarning("Already connected");
            }
            else if (cError == NetworkConnectionError.NATPunchthroughFailed)
            //NAT Punchthrough Failed
            {
                Debug.LogWarning("NAT Punchthrough Failed");
            }
        }

        public void OnConnectedToServer()
        {
            Debug.Log("Connected to server");
        }

        public void OnDisconnected()
        {
            Debug.LogWarning("Connection closed");
        }

        public void OnServerInitialized()
        {
            Debug.Log("Server initialized");
        }
    }

    public class Database : MonoBehaviour
    //Holds all methods related to database interactions, including 
    {
        public static string[] login(string email, string password)
        {
            string[] loginDetails = new string[3];
            WWWForm form = new WWWForm();
            form.AddField("user_email", email);
            form.AddField("user_password", password);
            WWW request = new WWW("liveworld.byethost22.com/501.php", form);
            while(!request.isDone)
            {
                if (request.isDone)
                {
                    char[] delimitter = new char[] { '&' };
                    loginDetails = request.text.Split(delimitter);
                }
            }
            request.Dispose();
            if (loginDetails[0].Length == 10)
            {
                return loginDetails;
            }
            else
            {
                return new string[] {"NLI"};
            }
        }
    }

    public class State : MonoBehaviour
    //Holds all methods and variables related to the state of the game (e.g. is the game paused?)
    {
        public static bool isPaused;
        //Use isPaused with caution. Online games are not meant to be paused. Provided only for versatility.
    }

    public class Synchronization : MonoBehaviour
    //Holds all methods related to server-client synchronization.
    {
        [RPC]
        public static void SynchronizePosition(Vector3 pos, Quaternion rot, Vector3 sca, GameObject go)
        {
            go.transform.position = pos;
            go.transform.rotation = rot;
            go.transform.localScale = sca;
        }
    }

    public class Communication : MonoBehaviour
    //Handles all communication-related methods, including chat message sending, parsing for channel symbols, etc.
    {
        public static void SendChatMessage(string messageToSend, string senderName)
        {
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            string channel = "";
            string receiverName = "";
            string message = "";
            char space = ' ';
            if (messageToSend.StartsWith("/p"))
            {
                channel = messageToSend.Remove(messageToSend.IndexOf(space));
                print(channel);
                string messageWithoutChannel = messageToSend.Replace("/p ", "");
                receiverName = messageWithoutChannel.Remove(messageWithoutChannel.IndexOf(space));
                print(receiverName);
                message = messageWithoutChannel.Replace(receiverName, "");
                print(message);

                foreach (GameObject po in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (po.GetComponent<PlayerComponent>().name == receiverName)
                    {
                        po.GetComponent<Chat>().chatMessages.Add("f " + senderName + ": " + message);
                    }
                    else if (po.GetComponent<PlayerComponent>().name == senderName)
                    {
                        po.GetComponent<Chat>().chatMessages.Add("t " + receiverName + ": " + message);
                    }
                }
            }
            else
            {
                foreach(GameObject po in GameObject.FindGameObjectsWithTag("Player"))
                {
                    po.GetComponent<Chat>().chatMessages.Add(senderName + ": " + messageToSend);
                }
            }

            
        }
    }
}
