using UnityEngine;
using System.Collections;

public class login_ui : MonoBehaviour {

    public bool loggedIn;
    public string name;
    public string pass;
    public string id;

    public string user_id;
    public string user_name;

    void OnGUI()
    {
        if (!loggedIn)
        {
            name = GUI.TextField(new Rect(0, 0, 200, 20), name);
            pass = GUI.PasswordField(new Rect(0, 25, 200, 20), pass, '*');
            if (GUI.Button(new Rect(0, 50, 200, 20), "Log In"))
            {
                if (name != "" && name != null && pass != "" && pass != null)
                {
                    StartCoroutine(doLogin(name, pass));
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
            pass = "";
        }
        else
        {
            print("NLI");
        }
    }
}
