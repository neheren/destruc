using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class sunRotater : NetworkBehaviour {
	public float speed;

	[SyncVar]
	float timeOfDay;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isServer) {
			transform.localRotation = Quaternion.Euler (timeOfDay, 100, 0);
			timeOfDay += speed;
		}
	}
}
