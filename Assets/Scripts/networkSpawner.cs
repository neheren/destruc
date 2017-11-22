using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class networkSpawner : NetworkBehaviour {

	public GameObject objectToSpawn;

	// Use this for initialization
	void Start () {
		CmdSpawn (objectToSpawn.name, this.transform.position);
		print (objectToSpawn.name);
	}

	[Command]
	void CmdSpawn (string obj, Vector3 pos) {
		GameObject objectToBeSpawned = (GameObject)Resources.Load (obj, typeof(GameObject));
		GameObject go = (GameObject)Instantiate (objectToBeSpawned, pos, Quaternion.Euler (0, 0, 0));

	}

}
