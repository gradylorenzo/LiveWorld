using UnityEngine;
using System.Collections;

public class login_ui : MonoBehaviour {

    public bool loggedIn;
    public string player_name;
    public string player_pass;
    public string player_id;

    public string user_id;
    public string user_name;

    void OnGUI()
    {
        if (!loggedIn)
        {
            player_name = GUI.TextField(new Rect(0, 0, 200, 20), player_name);
            player_pass = GUI.PasswordField(new Rect(0, 25, 200, 20), player_pass, '*');
            if (GUI.Button(new Rect(0, 50, 200, 20), "Log In"))
            {
                if (player_name != "" && player_name != null && player_pass != "" && player_pass != null)
                {
                    StartCoroutine(doLogin(player_name, player_pass));
                }
            }
        }
        else
        {
            GUI.Label(new Rect(0, 0, 300, 20), "Logged in as " + user_name + ", " + user_id);
        }
    }

    IEnumerator doLogin(string n, string p)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerName", n);
        form.AddField("playerPass", p);
        WWW w = new WWW("http://liveworld.byethost22.com/501.php", form);
        yield return w;

        
        if (w.text != "")
        {
            char[] delim = { '&' };
            string[] login = w.text.Split(delim);
            user_id = login[0];
            user_name = login[1];
            loggedIn = true;
            player_pass = "";
        }
        else
        {
            print("NLI");
        }
    }
}
