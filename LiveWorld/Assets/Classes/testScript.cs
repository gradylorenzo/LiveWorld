using UnityEngine;
using System.Collections;
using LiveWorld;

public class testScript : MonoBehaviour {

    // Use this for initialization

    public LWInterface.Notification newNotification;

	void Start () {
        newNotification = new LWInterface.Notification();
        newNotification.Text = "my notification";
	}
}
