using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnZone : MonoBehaviour {

	public int team;
	public gameController game;

	// Use this for initialization
	void Start () {
		//game = GameObject.FindWithTag ("controller").GetComponent<gameController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col) {
		if (col.transform.tag == "flag") {
			GameObject playerWhoCaptured = col.gameObject.GetComponent<flag> ().moveTransform.parent.gameObject;
			playerStats pStats = playerWhoCaptured.GetComponent<playerStats> ();
			if (pStats.team == team && pStats.isLocalPlayer) {
				game.CmdAnnounceFlag (pStats.playerName, pStats.team);
				col.gameObject.GetComponent<flag> ().dropFlag ();
			}
		}
	}
}
