using UnityEngine;
using System.Collections;

public class login_ui : MonoBehaviour {

    //Server
    public bool isServer;
    public GUISkin main_skin;
    public Texture2D background;
    //Player Information
    public string user_id;
    public string user_name;
    public string user_email;
    public string user_pass;

    //
    private bool LoggedIn = false;
    
    private string ConnectionMessage;
    private GameObject playerGO;

    //Exit Warning
    private bool ewShow = false;
    private float ewWantedY;
    private float ewCurrentY;
    private float ewLerpY;

    void Awake()
    {
        if (PlayerPrefs.HasKey("playeremail"))
        {
            user_email = PlayerPrefs.GetString("playeremail");
        }

        if (isServer)
        {
            bool useNat = !Network.HavePublicAddress();
            Network.InitializeServer(32, 25566, useNat);
        }
    }

    void Update()
    {
        if (ewLerpY < 1)
        {
            ewLerpY += 2 * Time.deltaTime;
        }

        if (ewCurrentY != ewWantedY)
        {
            ewCurrentY = Mathf.Lerp(ewCurrentY, ewWantedY, ewLerpY);
        }

        if (ewShow)
        {
            ewWantedY = 70;
        }
        else
        {
            ewWantedY = 0;
        }
    }

    void OnGUI()
    {
        GUI.skin = main_skin;
        //login UI
        if (!LoggedIn)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
            GUI.Box(new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 80, 320, 160), "Log into LiveWorld");
            GUILayout.BeginArea(new Rect((Screen.width / 2) - 155, (Screen.height / 2) - 50, 310, 160));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Email:");
            user_email = GUILayout.TextField(user_email, GUILayout.Width(225));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Password:");
            user_pass = GUILayout.PasswordField(user_pass, '*', GUILayout.Width(225));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Log In"))
            {
                //Login Attempt responses
                if (user_email == "" && user_pass == "")
                {
                    ConnectionMessage = "Email and Password required";
                }
                else if (user_email == ""){
                    ConnectionMessage = "Email required";
                }
                else if (user_pass == "")
                {
                    ConnectionMessage = "Password required";
                }
                else if (user_email != "" && user_pass != "")
                {
                    //Attempt to log in if both email and password are provided
                    StartCoroutine(doLogin(user_email, user_pass));
                    ConnectionMessage = "Connecting to Database";
                    user_pass = "";
                }
            }
            if (GUILayout.Button("Exit"))
            {
                ewShow = true;
                ewLerpY = 0;
            }
            GUILayout.Label(ConnectionMessage);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        
            //Exit Warning
            GUI.Box(new Rect(Screen.width / 2 - 160, Screen.height - ewCurrentY, 320, 65), "Are you sure you want to exit?");
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 155, Screen.height - ewCurrentY + 30, 310, 60));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("No"))
            {
                ewShow = false;
                ewLerpY = 0;
            }
            if (GUILayout.Button("Yes"))
            {
                print("closing");
                Application.Quit();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        
        }
        else
        {
            ewShow = false;
        }
    }

    void OnFailedToConnect()
    {
        ConnectionMessage = "Failed to connect to master server";
    }

    void OnConnectedToServer()
    {
        ConnectionMessage = "Connected! Spawning " + user_name;
        LoggedIn = true;
        StartCoroutine(doReserveInstance(user_id));

        //Spawn the player
        playerGO = Resources.Load("PlayerPrefab") as GameObject;
        playerGO.GetComponent<playerNetworking>().playerName = user_name;
        playerGO.GetComponent<playerNetworking>().playerID = user_id;
        Network.Instantiate(playerGO, transform.position, transform.rotation, 0);
        playerGO = null;
        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
    }

    void OnDisconnectedFromServer()
    {
        ConnectionMessage = "Disconnected";
        LoggedIn = false;
        StartCoroutine(doUnreserveInstance(user_id));
        GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
    }

    void OnApplicationQuit()
    {
        ConnectionMessage = "Disconnected";
        LoggedIn = false;
        if (LoggedIn)
        {
            StartCoroutine(doUnreserveInstance(user_id));
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        if (Network.isServer)
        {
            print("Clean up after player " + player);
            Network.RemoveRPCs(player);
            Network.DestroyPlayerObjects(player);
        }
    }
    
    IEnumerator doLogin(string e, string p)
    {
        //Create a new form for the WWW request
        WWWForm loginForm = new WWWForm();
        loginForm.AddField("email", e);
        loginForm.AddField("pass", p);

        //Make the WWW request using the form
        WWW w = new WWW("liveworld.byethost22.com/501.php",loginForm);

        //wait for information download to complete
        yield return w;

        //Check to see if the login is valid
        if (w.text != "")
        {
            char[] delim = {'&'};
            string[] wTextArray = w.text.Split(delim);
            
            //is the user already logged into another instance?
            if (wTextArray[2] == "true")
            {
                //user is logged into another instance. notify the user and do not log in
                ConnectionMessage = "Already logged into another instance";
            }
            else
            {
                //user is not logged into an instance, connect to the master server
                ConnectionMessage = "Connecting to master server";
                user_id = wTextArray[0];
                user_name = wTextArray[1];
                Network.Connect("52.11.93.30", 25566);
                PlayerPrefs.SetString("playeremail", user_email);
            }
        }
        else
        {
            ConnectionMessage = "Incorrect credentials";
        }
    }

    IEnumerator doReserveInstance(string id)
    {
        WWWForm loggedInForm = new WWWForm();
        loggedInForm.AddField("user_id", id);
        WWW w = new WWW("liveworld.byethost22.com/502.php", loggedInForm);

        yield return w;
    }

    IEnumerator doUnreserveInstance(string id)
    {
        WWWForm loggedInForm = new WWWForm();
        loggedInForm.AddField("user_id", id);
        WWW w = new WWW("liveworld.byethost22.com/503.php", loggedInForm);

        yield return w;
    }
}
