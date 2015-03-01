using UnityEngine;
using System.Collections;

public class connection_test : MonoBehaviour {

    public bool server = false;

    void Start()
    {
        if (server)
        {
            bool useNat = !Network.HavePublicAddress();
            Network.InitializeServer(32, 25566, useNat);
        }
        else
        {
            Network.Connect("52.10.70.115", 25566);
        }
    }

    void OnConnectedToServer()
    {
        print("connected!");
    }

    void OnFailedToConnectToServer()
    {
        print("failed to connect!");
    }
}
