using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace LiveWorld
{
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
                if (NetworkMode == LWNetworkModes.dev_server)
                {
                    NetworkManager.singleton.networkPort = 7778;
                    NetworkManager.singleton.StartServer();
                    LWInterface.NewNotification("Dev server ready", LWInterface.Notification.LWNotificationType.Success);
                }
                else if (NetworkMode == LWNetworkModes.rel_server)
                {
                    NetworkManager.singleton.networkPort = 7777;
                    NetworkManager.singleton.StartServer();
                    LWInterface.NewNotification("Rel server ready", LWInterface.Notification.LWNotificationType.Success);
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
}
