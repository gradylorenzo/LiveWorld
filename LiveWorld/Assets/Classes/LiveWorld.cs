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

    public class LWTime : MonoBehaviour
    {
        public static int SecondOfDay = 0;
        public static int MinuteOfDay = 0;
        public static int HourOfDay = 0;

        public static int Second = 0;
        public static int Minute = 0;
        public static int Hour = 0;

        public static float TimeScale = 1;

        void Awake()
        {
            TimeScale = Mathf.Clamp(TimeScale, .1f, 100);
            InvokeRepeating("TimeStep", 0, 1 / TimeScale);
        }

        void TimeStep()
        {
            if (SecondOfDay < 86399)
            {
                SecondOfDay += 1;
            }
            else
            {
                SecondOfDay = 0;
            }
        }

        void LateUpdate()
        {
            MinuteOfDay = SecondOfDay / 60;
            HourOfDay = MinuteOfDay / 60;

            Second = SecondOfDay - (MinuteOfDay * 60);
            Minute = MinuteOfDay - (HourOfDay * 60);
            Hour = SecondOfDay / 3600;
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
        public static bool showHomeBar = false;

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
                currentX = Screen.width / 3;
                currentY = -30;
                wantedX = Screen.width / 3;
                wantedY = 5;

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("LWNotificationObject"))
                {
                    if (go != this.gameObject)
                    {
                        go.GetComponent<Notification>().wantedY += 30;
                    }
                }
            }

            void OnGUI()
            {
                GUI.Box(new Rect(Screen.width - currentX, currentY, (Screen.width / 3) - 5, 25), "");
                GUI.Label(new Rect(Screen.width - currentX + 5, currentY + 2, (Screen.width / 3), 20), Text);

                currentX = Mathf.Lerp(currentX, wantedX, .1f);
                currentY = Mathf.Lerp(currentY, wantedY, .1f);

                if (currentX <= 0)
                {
                    Destroy(this.gameObject);
                }

                if(currentY >= Screen.height / 2)
                {
                    clearNotification();
                }
            }

            void clearNotification()
            {
                wantedX = -10;
            }
        }


        //HomeBar functionality
        public class HomeBar : MonoBehaviour
        {
            public static float wantedY = 0;
            private static float currentY = 0;
            public static bool isShowing = false;

            public static void OnGUI()
            {
                if (isShowing)
                {
                    wantedY = 40;
                }
                else
                {
                    wantedY = 0;
                }

                currentY = Mathf.Lerp(currentY, wantedY, .1f);

                GUILayout.BeginArea(new Rect(0, Screen.height - currentY, Screen.width, 40));
                GUILayout.BeginHorizontal();
                GUILayout.Button("Profile");
                GUILayout.Button("Social");
                GUILayout.Button("World");
                GUILayout.Button("Settings");
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            public static void Toggle()
            {
                isShowing = !isShowing;
            }
        }

        //Function to call when creating a new notification
        public static void NewNotification(string Text, Notification.NotificationType Type)
        {
            GameObject newNotification = Resources.Load("LWNotificationObject") as GameObject;
            newNotification.GetComponent<Notification>().Text = Text;
            newNotification.GetComponent<Notification>().Type = Type;
            Instantiate(newNotification, Vector3.zero, new Quaternion(0, 0, 0, 0));
            newNotification = null;
        }
    }

    public class LWWeather : MonoBehaviour
    {
        public enum WeatherTypes
        {
            clear,
            cloudy,
            rain,
        }

        public static AnimationCurve TemperatureCurve;
        public static AnimationCurve RainChanceCurve;

        public static int Temperature;
        public static bool isRaining;

        public static void DetermineNewTemperature()
        {

        }

        public static void DetermineIsRaining()
        {

        }
    }
}
