using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class grabMoney : NetworkBehaviour {

	int value = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col) {
		if (col.transform.tag == "player") {
			GameObject player = col.gameObject;
			player.GetComponent<playerStats> ().money += value;
			player.GetComponent<hit> ().CmdDespawn (netId);
		}
	}
}
