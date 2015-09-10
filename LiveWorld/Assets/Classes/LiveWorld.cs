using UnityEngine;
using System.Collections;
using System;

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

    public class LWServer : MonoBehaviour
    {
        public enum ServerTypes
        {
            development,
            live,
            not_server
        }

        public static ServerTypes ServerType = ServerTypes.not_server;

        //Pending changes per changes in Unity 5.2 networking system.
        public static void InitializeServer(int port, int limit, string password, bool usePassword, bool useNAT)
        {
            if (usePassword)
            {
                Network.incomingPassword = password;
            }
            Network.InitializeServer(limit, port, useNAT);
        }

        public class NPCManager
        {
            public void SpawnNPC(GameObject npcToSpawn)
            {

            }
        }
    }

    public class LWClient : MonoBehaviour
    {
        public enum ClientTypes
        {
            development,
            live
        }

        public static ClientTypes ClientType = ClientTypes.development;

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
            LWInterface.NewNotification("Attempting to connect..", LWInterface.Notification.NotificationType.message);
        }

        public static IEnumerator DoLogin()
        {
            if (Details.email != null && Details.password != null)
            {

                WWWForm loginForm = new WWWForm();
                loginForm.AddField("email", Details.email);
                loginForm.AddField("password", Details.password);

                //------DOMAIN FOR LOGGING IN
                string loginDomain = "http://www.tethys-edu.com/501.php";
                //---------------------------

                WWW request = new WWW(loginDomain, loginForm);

                yield return request;

                string returnedText = request.text;
                string[] details;

                if (request.isDone)
                {
                    if (returnedText != "NLI")
                    {
                        char delimitter = '&';
                        details = returnedText.Split(delimitter);
                        Details.SetUserCredentials(details[0], details[1]);
                    }
                    else
                    {
                        LWInterface.NewNotification("Incorrect Credentials.", LWInterface.Notification.NotificationType.error);
                    }
                }
            }
            else
            {
                Debug.LogWarning("LWCLientDetail: email and password not set");
            }
        }

        public class Details : MonoBehaviour
        {
            public static bool loggedIn = false;
            public static string userID = "";
            public static string username = "";
            public static string email = "";
            public static string password = "";

            public static void SetLoginCredentials(string e, string p)
            {
                email = e;
                password = p;
            }

            public static void SetUserCredentials(string i, string u)
            {
                userID = i;
                username = u;
                email = "";
                password = "";
                loggedIn = true;
                LWInterface.NewNotification("Successfuly logged in", LWInterface.Notification.NotificationType.success);
                print(username + " :" + userID + ": " + loggedIn);
                if (LWClient.ClientType == LWClient.ClientTypes.development)
                {
                    LWClient.ConnectToServer("54.148.244.243", 25567, "", false);
                }
                else if (LWClient.ClientType == LWClient.ClientTypes.live)
                {
                    LWClient.ConnectToServer("54.148.244.243", 25566, "", false);
                }
            }

            public static void ClearCredentials()
            {
                userID = "";
                username = "";
                email = "";
                password = "";
            }
        }
    }

    public class LWSettings
    {
        public class LWAudio
        {
            public enum AudioTypes
            {
                sfx,
                voice,
                ui,
                atmosphere,
                music
            }

            public static int sfx;
            public static int voice;
            public static int ui;
            public static int atmosphere;
            public static int music;
        }

        public class LWVideo
        {

        }

        public class LWControl
        {
            public static int mouseSensitivity;
        }

        public static void CreateDefaultSettings ()
        {
            PlayerPrefs.SetInt("firstrun", 1);
            PlayerPrefs.SetInt("audio.sfx", 5);
            PlayerPrefs.SetInt("audio.voice", 5);
            PlayerPrefs.SetInt("audio.ui", 5);
            PlayerPrefs.SetInt("audio.atmosphere", 5);
            PlayerPrefs.SetInt("audio.music", 5);

            PlayerPrefs.SetInt("control.mouseSensitivity", 5);
        }

        public static void RetrieveSettings()
        {
            if (PlayerPrefs.HasKey("firstrun"))
            {
                LWAudio.sfx = PlayerPrefs.GetInt("audio.sfx");
                LWAudio.voice = PlayerPrefs.GetInt("audio.voice");
                LWAudio.voice = PlayerPrefs.GetInt("audio.ui");
                LWAudio.voice = PlayerPrefs.GetInt("audio.atmosphere");
                LWAudio.voice = PlayerPrefs.GetInt("audio.music");

                LWControl.mouseSensitivity = PlayerPrefs.GetInt("control.mouseSensitivity");
            }
            else
            {
                CreateDefaultSettings();
            }
        }
    }

    public class LWInterface : MonoBehaviour
    {
        public static GUISkin ui_skin;

        void Update()
        {
            ui_skin.button.fixedWidth = (Screen.width / 4) + 4;
        }

        public class Notification : MonoBehaviour
        {
            public string Text;
            public float Duration = 5;

            public enum NotificationType
            {
                message,
                warning,
                error,
                success
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
                GUI.skin = ui_skin;

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
        public class HomeBar
        {
            public static float wantedY = 0;
            private static float currentY = 0;
            public static bool isShowing = false;

            public static void OnGUI()
            {
                if (isShowing)
                {
                    wantedY = 25;
                }
                else
                {
                    wantedY = 0;
                }

                currentY = Mathf.Lerp(currentY, wantedY, .1f);

                GUILayout.BeginArea(new Rect(0, Screen.height - currentY, Screen.width, 40));
                GUILayout.BeginHorizontal();
                GUILayout.Button(LWClient.Details.username.ToUpper());
                GUILayout.Button("SOCIAL");
                GUILayout.Button("WORLD");
                GUILayout.Button("SETTINGS");
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
        public class Precipitation
        {
            public enum PrecipitationTypes
            {
                clear,
                rain,
            }

            public static PrecipitationTypes Current;

            public static int RainChance = 10;
            private static int randomSeed;

            public static void DetermineIsRaining()
            {
                randomSeed = UnityEngine.Random.Range(0, 99);

                if (randomSeed <= RainChance)
                {
                    Current = PrecipitationTypes.rain;
                }
                else
                {
                    Current = PrecipitationTypes.clear;
                }
                print(Current + "  " + randomSeed);
            }
        }

        public class Temperature
        {
            public static AnimationCurve TemperatureCurve;
            public static int Current;

            public static void DetermineNewTemperature()
            {
                Current = (int)TemperatureCurve.Evaluate(LWTime.HourOfDay);
            }
        }
    }

    [Obsolete("LWPlayer class pending changes per changes in Unity 5.2 networking system.", false)]
    public class LWPlayer : MonoBehaviour
    {
        //Pending changes per changes in Unity 5.2 networking system.
        [RequireComponent(typeof(NetworkView))]

        [System.Serializable]
        public class PlayerInformation
        {
            public string name;
            public int level;
        }

        [System.Serializable]
        public class PlayerStats
        {
            public int health = 100;
            public int attack = 1;
            public int defense = 1;
            public int speed = 1;
        }

        private NetworkView nView;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        public float syncThreshold;
        public PlayerStats Statistics;

        void Awake()
        {
            nView = new NetworkView();
        }

        void Update()
        {
            
            if (GetComponent<NetworkView>().isMine)
            {
                if (Vector3.Distance(this.transform.position, lastPosition) > syncThreshold || Quaternion.Angle(this.transform.rotation, lastRotation) > syncThreshold)
                {
                    nView.RPC("SynchronizePosition", RPCMode.OthersBuffered, this.transform.position, this.transform.rotation);
                }
            }
        }

        [RPC]
        void SynchronizePosition(Vector3 newPosition, Quaternion newRotation)
        {
            this.transform.position = newPosition;
            this.transform.rotation = newRotation;
        }
    }
}
