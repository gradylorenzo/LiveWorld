using UnityEngine;
using System.Collections;

namespace LiveWorld
{
    public class LWTestFunctions : MonoBehaviour
    {
        public static void test()
        {
            print("Test string");
        }
    }

    public class LWServerMethod
    {
        public static void InitializeServer(int port, int limit, string password, bool usePassword, bool useNAT)
        {
            if (usePassword)
            {
                Network.incomingPassword = password;
            }
            Network.InitializeServer(limit, port, useNAT);
        }
    }

    public class LWClientMethod
    {
        public static void ConnectToServer(string ipAddress, int port, string password, bool usePassword)
        {
            if (usePassword)
            {
                Network.Connect(ipAddress, port, password);
            }
            else
            {
                Network.Connect(ipAddress, port);

            }
        }
    }

    public class LWInterface : MonoBehaviour
    {
        public class Notification : MonoBehaviour
        {
            public string Text;
            public float Duration = 5;

            public enum NotificationType
            {
                message,
                warning,
                error
            }

            private float currentX;
            private float currentY;
            public float wantedX;
            public float wantedY;

            public NotificationType Type;

            void Start()
            {
                Invoke("clearNotification", Duration);
                currentX = 5;
                currentY = Screen.height;
                wantedX = 5;
                wantedY = Screen.height - 30;

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("LWNotificationObject"))
                {
                    if(go != this.gameObject)
                    {
                        go.GetComponent<Notification>().wantedY -= 30;
                    }
                }
            }

            void OnGUI()
            {
                GUI.Box(new Rect(currentX, currentY, 300, 25), "");
                GUI.Label(new Rect(currentX + 5, currentY + 5, 295, 20), Text);

                currentX = Mathf.Lerp(currentX, wantedX, .1f);
                currentY = Mathf.Lerp(currentY, wantedY, .1f);

                if(currentX <= -300)
                {
                    Destroy(this.gameObject);
                }
            }

            void clearNotification()
            {
                wantedX = -400;
            }
        }

        //Function to call when creating a new notification
        public static void NewNotification(string Text, Notification.NotificationType Type)
        {
            GameObject newNotification = Resources.Load("LWNotificationObject") as GameObject;
            newNotification.GetComponent<Notification>().Text = Text;
            newNotification.GetComponent<Notification>().Type = Type;
            Instantiate(newNotification, Vector3.zero, new Quaternion(0, 0, 0, 0));
        }
    }
}