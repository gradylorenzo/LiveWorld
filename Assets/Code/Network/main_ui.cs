using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class main_ui : MonoBehaviour {

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
    public struct uiBooleans
    {
        public bool showHomeBar;
        public bool showChat;
        public bool showSettings;
    }

    public homeBarRects homeBarRect;
    public settingsRects settingsRect;
    public uiBooleans uiBooleanSet;

    void Start()
    {
        settingsRect.currentX = -(Screen.width / 4) - 5;
        settingsRect.wantedX = -(Screen.width / 4) - 5;
    }

    void OnGUI()
    {
        GUI.skin = main_skin;
        main_skin.customStyles[0].fixedWidth = Screen.width / 4;

        GUILayout.BeginArea(new Rect(0, (Screen.height - homeBarRect.currentHeight), Screen.width, 30));
        GUILayout.BeginHorizontal();
        GUILayout.Button("PROFILE", main_skin.customStyles[0]);
        GUILayout.Button("SOCIAL", main_skin.customStyles[0]);
        GUILayout.Button("WORLD", main_skin.customStyles[0]);
        if (GUILayout.Button("SETTINGS", main_skin.customStyles[0]))
        {
            if (uiBooleanSet.showSettings)
            {
                uiBooleanSet.showSettings = false;
            }
            else
            {
                uiBooleanSet.showSettings = true;
            }
            settingsRect.lerpX = 0;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //Settings window
        GUILayout.BeginArea(new Rect(settingsRect.currentX, 5, Screen.width / 4, Screen.height / 2), main_skin.box);
        GUILayout.BeginVertical();
        if (GUILayout.Button("Log Out"))
        {
            Network.Disconnect();
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

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
                settingsRect.lerpX = 0;
            }
            else
            {
                uiBooleanSet.showHomeBar = true;
                homeBarRect.lerpHeight = 0;
                settingsRect.lerpX = 0;
            }
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
    }
}
