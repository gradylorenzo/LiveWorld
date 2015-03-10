using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class main_ui : MonoBehaviour {

    private NetworkView nView;
    public GUISkin main_skin;
    public Texture2D crosshair;

    public struct homeBarRects
    {
        public float wantedHeight;
        public float currentHeight;
        public float lerpHeight;
    }
    public struct settingsRects
    {
        public float wantedX;
        public float currentX;
        public float lerpX;
    }
    public struct chatRects
    {
        public float wantedX;
        public float currentX;
        public float lerpX;
    }
    public struct uiBooleans
    {
        public bool showHomeBar;
        public bool showChat;
        public bool showSettings;
    }

    public homeBarRects homeBarRect;
    public settingsRects settingsRect;
    public chatRects chatRect;
    public uiBooleans uiBooleanSet;
    public string chatMessage;
    public List<string> chatMessagesList = new List<string>();
    public Vector2 chatScrollPosition;

    void Start()
    {
        settingsRect.currentX = -(Screen.width / 4) - 5;
        settingsRect.wantedX = -(Screen.width / 4) - 5;
        chatRect.currentX = -(Screen.width / 4) - 5;
        chatRect.wantedX = -(Screen.width / 4) - 5;

        nView = GetComponent<NetworkView>();
    }

    void OnGUI()
    {
        GUI.skin = main_skin;
        main_skin.customStyles[0].fixedWidth = Screen.width / 4 + 1;

        GUILayout.BeginArea(new Rect(0, (Screen.height - homeBarRect.currentHeight), Screen.width, 30));
        GUILayout.BeginHorizontal();
        GUILayout.Button(GetComponent<playerNetworking>().playerName, main_skin.customStyles[0]);
        if (GUILayout.Button("Social", main_skin.customStyles[0]))
        {
            if (uiBooleanSet.showChat)
            {
                uiBooleanSet.showChat = false;
            }
            else
            {
                uiBooleanSet.showChat = true;
                uiBooleanSet.showSettings = false;
            }
            settingsRect.lerpX = 0;
            chatRect.lerpX = 0;
        }
        GUILayout.Button("World", main_skin.customStyles[0]);
        if (GUILayout.Button("Settings", main_skin.customStyles[0]))
        {
            if (uiBooleanSet.showSettings)
            {
                uiBooleanSet.showSettings = false;
            }
            else
            {
                uiBooleanSet.showSettings = true;
                uiBooleanSet.showChat = false;
            }
            settingsRect.lerpX = 0;
            chatRect.lerpX = 0;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //Settings window
        if (settingsRect.currentX > -(Screen.width / 4) - 5)
        {
            GUILayout.BeginArea(new Rect(settingsRect.currentX, 5, Screen.width / 4, Screen.height / 2), main_skin.box);
            GUILayout.BeginVertical();
            GUILayout.Label("Settings");
            if (GUILayout.Button("Log Out"))
            {
                Network.Disconnect();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        //Chat window
        if (chatRect.currentX > -(Screen.width / 4) - 5)
        {
            GUILayout.BeginArea(new Rect(chatRect.currentX, 5, Screen.width / 4, Screen.height / 2), main_skin.box);
            GUILayout.BeginVertical();
            GUILayout.Label("Chat", GUILayout.Width(Screen.width / 4 - 10));
            chatScrollPosition = GUILayout.BeginScrollView(chatScrollPosition, false, true);
            foreach (string s in chatMessagesList)
            {
                GUILayout.Label(s, main_skin.customStyles[2]);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(chatRect.currentX, Screen.height / 2 + 10, Screen.width / 4, 30), main_skin.box);
            GUILayout.BeginHorizontal();
            chatMessage = GUILayout.TextField(chatMessage, GUILayout.Width(Screen.width / 4 - 60));
            if(GUILayout.Button("Send", GUILayout.Width(50)))
            {
                compileChatMessageAndSend();
            }
            if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
            {
                compileChatMessageAndSend();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        //clock

        if (!uiBooleanSet.showHomeBar)
        {
            GUI.DrawTexture(new Rect((Screen.width / 2) - 20, (Screen.height / 2) - 20, 40, 40), crosshair);
        }
    }
    void Update()
    {
        //Toggle UI
        if (Input.GetButtonDown("TOGGLE_UI"))
        {
            if (uiBooleanSet.showHomeBar)
            {
                uiBooleanSet.showHomeBar = false;
                homeBarRect.lerpHeight = 0;
            }
            else
            {
                uiBooleanSet.showHomeBar = true;
                homeBarRect.lerpHeight = 0;
            }
            settingsRect.lerpX = 0;
            chatRect.lerpX = 0;
        }

        //Homebar lerp
        if (uiBooleanSet.showHomeBar)
        {
            homeBarRect.wantedHeight = 30;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GetComponent<FirstPersonController>().enabled = false;
        }
        else
        {
            homeBarRect.wantedHeight = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GetComponent<FirstPersonController>().enabled = true;
        }

        if (homeBarRect.currentHeight != homeBarRect.wantedHeight)
        {
            homeBarRect.currentHeight = Mathf.Lerp(homeBarRect.currentHeight, homeBarRect.wantedHeight, homeBarRect.lerpHeight);
        }

        if (homeBarRect.lerpHeight < 1)
        {
            homeBarRect.lerpHeight += 2 * Time.deltaTime;
        }

        //Settings lerp
        if (uiBooleanSet.showSettings && uiBooleanSet.showHomeBar)
        {
            settingsRect.wantedX = 5;
        }
        else
        {
            settingsRect.wantedX = -(Screen.width / 4) - 5;
        }

        if (settingsRect.lerpX < 1)
        {
            settingsRect.lerpX += 2 * Time.deltaTime;
        }

        if (settingsRect.currentX != settingsRect.wantedX)
        {
            settingsRect.currentX = Mathf.Lerp(settingsRect.currentX, settingsRect.wantedX, settingsRect.lerpX);
        }
        
        //Chat lerp
        if (uiBooleanSet.showChat && uiBooleanSet.showHomeBar)
        {
            chatRect.wantedX = 5;
        }
        else
        {
            chatRect.wantedX = -(Screen.width / 4) - 5;
        }

        if (chatRect.lerpX < 1)
        {
            chatRect.lerpX += 2 * Time.deltaTime;
        }

        if (chatRect.currentX != chatRect.wantedX)
        {
            chatRect.currentX = Mathf.Lerp(chatRect.currentX, chatRect.wantedX, chatRect.lerpX);
        }
    }

    void compileChatMessageAndSend()
    {
        if (chatMessage != "")
        {
            string messageToSend = GetComponent<playerNetworking>().playerName + ": " + chatMessage;
            nView.RPC("RecieveChatMessage", RPCMode.All, messageToSend);
            messageToSend = null;
            chatMessage = "";
        }
    }

    [RPC]
    void RecieveChatMessage(string m)
    {
        GameObject[] allChatObjects;
        allChatObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in allChatObjects)
        {
            if (go.GetComponent<main_ui>().enabled)
            {
                go.GetComponent<main_ui>().chatMessagesList.Add(m);
                go.GetComponent<main_ui>().chatScrollPosition.y += 10000000;
            }
        }
    }
}
