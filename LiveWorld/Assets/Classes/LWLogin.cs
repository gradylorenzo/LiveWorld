using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace LiveWorld
{
    public class LWLogin
    {
        public enum LWLoginMode
        {
            manual_connection,
            automatic_connection
        }

        public static LWLoginMode LoginMode = LWLoginMode.automatic_connection;
        public static bool isLoggedIn = false;
        private static string Domain = "http://tethys-edu.com/501.php";


        //Credentials used by the login system
        public static class Credentials
        {
            public static string user_ID;
            public static string user_name;
            public static string user_email;
            public static string user_password;

            public static void ClearLogin()
            {
                user_email = "";
                user_password = "";
            }

            public static void ClearUser()
            {
                user_ID = "";
                user_name = "";
            }

            public static void SetLogin(string email, string password)
            {
                user_email = email;
                user_password = password;
            }

            public static void SetUser(string id, string name)
            {
                user_ID = id;
                user_name = name;
            }
        }

        public static IEnumerator Login()
        {

            if (Credentials.user_email != "" && Credentials.user_password != "")
            {
                LWInterface.NewNotification("Attempting to log in..", LWInterface.Notification.LWNotificationType.Message);
                WWWForm requestForm = new WWWForm();
                requestForm.AddField("email", Credentials.user_email);
                requestForm.AddField("password", Credentials.user_password);
                WWW request = new WWW(Domain, requestForm);

                yield return request;

                string requestText = request.text;

                if (requestText != "NLI")
                {
                    string[] creds;
                    char delim = '&';
                    creds = requestText.Split(delim);

                    Credentials.SetUser(creds[0], creds[1]);
                    isLoggedIn = true;

                    if (LoginMode == LWLoginMode.automatic_connection)
                    {
                        LWInterface.NewNotification(Credentials.user_name + " logged in. Connecting...", LWInterface.Notification.LWNotificationType.Success);
                        LWNetwork.Client.InitializeClient();
                    }
                    else
                    {
                        LWInterface.NewNotification(Credentials.user_name + " logged in.", LWInterface.Notification.LWNotificationType.Success);
                    }
                    //Automatically connect after logging in if configured to do so
                }
                else
                {
                    LWInterface.NewNotification("Incorrect login.", LWInterface.Notification.LWNotificationType.Error);
                    //Do something else if the login is invalid
                }
                //If the page does not return "NLI", the login is valid
                //Otherwise, the incorrect credentials were provided

                Credentials.ClearLogin();
                //Clear the user's login credentials, regardless of their validity
            }
            else
            {
                Debug.LogError("Login credentials not set, use LWLogin.Credentials.SetLogin()");
            }
        }

        public static void Logout()
        {
            Credentials.user_email = "";
            Credentials.user_ID = "";
            Credentials.user_name = "";
            Credentials.user_password = "";
            isLoggedIn = false;
        }
    }
    //-------------------------------------------------------------------------------------------------------
}
