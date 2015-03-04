using UnityEngine;
using System.Collections;

public class login_ui : MonoBehaviour {

    public bool isServer;

    public bool loggedIn;
    public string player_email;
    public string player_pass;
    public string player_id;

    public string user_id;
    public string user_name;

    public GameObject playerPrefab;
    private bool user_exists;

    void Start()
    {
        if (isServer)
        {
            bool useNat = !Network.HavePublicAddress();
            Network.InitializeServer(32, 25566, useNat);
        }
    }

    void OnGUI()
    {
        if (!loggedIn && !Network.isServer)
        {
            player_email = GUI.TextField(new Rect(0, 0, 200, 20), player_email);
            player_pass = GUI.PasswordField(new Rect(0, 25, 200, 20), player_pass, '*');
            if (GUI.Button(new Rect(0, 50, 200, 20), "Log In"))
            {
                if (player_email != "" && player_email != null && player_pass != "" && player_pass != null)
                {
                    StartCoroutine(doLogin(player_email, player_pass));
                }
            }
        }
        else
        {
            GUI.Label(new Rect(0, 0, 400, 20), "Logged in as " + user_name + ", " + user_id);
            if (GUI.Button(new Rect(0, 25, 200, 20), "Log off"))
            {
                Network.Disconnect();
            }
        }
    }

    IEnumerator doLogin(string e, string p)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerEmail", e);
        form.AddField("playerPass", p);
        WWW w = new WWW("http://liveworld.byethost22.com/501.php", form);
        yield return w;

        
        if (w.text != "")
        {
            char[] delim = { '&' };
            string[] login = w.text.Split(delim);
            user_id = login[0];
            user_name = login[1];
            player_pass = "";
            print("player information retrieved.");
            connectToServer();
        }
        else
        {
            print("NLI");
        }
    }

    void connectToServer()
    {
        Network.Connect("52.11.93.30", 25566);
        print("connecting to server.");
    }

    void OnConnectedToServer()
    {
        if (user_name != "")
        {
            GameObject[] otherplayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject go in otherplayers)
            {
                if (go.GetComponent<player_networking>().player_name == user_name)
                {
                    user_exists = true;
                    Network.Disconnect();
                    print("user already connected in another instance, disconnecting");
                }
            }

            if (!user_exists)
            {
                print("logged in successfully, spawning player " + user_name);
                playerPrefab = Resources.Load("PlayerPrefab") as GameObject;
                playerPrefab.GetComponent<player_networking>().player_name = user_name;
                Network.Instantiate(playerPrefab, transform.position, transform.rotation, 0);
                playerPrefab = null;
                loggedIn = true;
                this.GetComponentInChildren<Camera>().enabled = false;
                this.GetComponentInChildren<AudioListener>().enabled = false;
            }
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    void OnDisconnectedFromServer()
    {
        player_pass = "";
        GameObject[] playerObjects;
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerObjects)
        {
            DestroyObject(go);
        }
        loggedIn = false;
        this.GetComponentInChildren<Camera>().enabled = true;
        this.GetComponentInChildren<AudioListener>().enabled = true;
    }
}