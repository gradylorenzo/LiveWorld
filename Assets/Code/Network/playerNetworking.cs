using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
public class playerNetworking : MonoBehaviour {

    public string playerName;
    public string playerID;

    [System.Serializable]
    public struct Synchronization
    {
        public float syncThreshold;
        public Vector3 lastPosition;
        public Quaternion lastRotation;
    }

    public Synchronization SyncSettings;

    private NetworkView nView;

    void Awake()
    {
        nView = GetComponent<NetworkView>();
        if (nView.isMine)
        {
            GetComponent<FirstPersonController>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
            nView.RPC("syncPlayerName", RPCMode.OthersBuffered, playerName, playerID);
            nView.RPC("syncPlayerPosition", RPCMode.OthersBuffered, transform.position, transform.rotation);
        }
    }

    void Update()
    {
        if (nView.isMine)
        {
            if (Vector3.Distance(transform.position, SyncSettings.lastPosition) >= SyncSettings.syncThreshold && Quaternion.Angle(transform.rotation, SyncSettings.lastRotation) >= SyncSettings.syncThreshold)
            {
                nView.RPC("syncPlayerPosition", RPCMode.OthersBuffered, transform.position, transform.rotation);
                SyncSettings.lastPosition = transform.position;
                SyncSettings.lastRotation = transform.rotation;
            }
        }
    }

    void OnPlayerConnected()
    {
        nView.RPC("syncPlayerName", RPCMode.AllBuffered, playerName, playerID);
        nView.RPC("syncPlayerPosition", RPCMode.OthersBuffered, transform.position, transform.rotation);
    }

    [RPC]
    void syncPlayerName(string n, string id)
    {
        this.playerName = n;
        this.playerID = id;
    }

    [RPC]
    void syncPlayerPosition(Vector3 p, Quaternion r)
    {
        this.transform.position = p;
        this.transform.rotation = r;
    }

    void OnDisconnectedFromServer()
    {
        Destroy(this.gameObject);
    }
}
