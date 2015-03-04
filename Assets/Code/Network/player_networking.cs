using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class player_networking : MonoBehaviour {

    public float syncThreshold;
    public Vector3 lastPosition;
    public Quaternion lastRotation;
    private NetworkView nView;

    public string player_name;

    void Awake()
    {
        nView = GetComponent<NetworkView>();
        if (nView.isMine)
        {
            this.GetComponentInChildren<Camera>().enabled = true;
            this.GetComponentInChildren<AudioListener>().enabled = true;
            this.GetComponent<CharacterController>().enabled = true;
            this.GetComponent<FirstPersonController>().enabled = true;
        }
    }

    void Update()
    {
        if (Network.isClient)
        {
            if (Vector3.Distance(lastPosition, transform.position) >= syncThreshold)
            {
                nView.RPC("syncPosition", RPCMode.OthersBuffered, transform.position, transform.rotation);
            }

            if (Quaternion.Angle(lastRotation, transform.rotation) >= syncThreshold * 10)
            {
                nView.RPC("syncPosition", RPCMode.OthersBuffered, transform.position, transform.rotation);
            }
        }
    }

    void OnPlayerConnectedToServer()
    {
        nView.RPC("syncPosition", RPCMode.OthersBuffered, transform.position, transform.rotation);
    }

    void OnDisconnectedFromServer()
    {
        Destroy(this);
    }

    [RPC]
    void syncPosition(Vector3 p, Quaternion r)
    {
        transform.position = p;
        transform.rotation = r;
    }
}
