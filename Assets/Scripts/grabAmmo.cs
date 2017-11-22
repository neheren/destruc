using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class grabAmmo : NetworkBehaviour {

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
			hit hitS = player.GetComponent<hit> ();
			Transform camera = player.transform.GetChild (1).transform;
			GameObject gun = null;
			for (int i = 0; i < camera.childCount; i++) {
				if (camera.GetChild (i).name == "gun") {
					gun = camera.GetChild (i).gameObject;
					break;
				}
			}

			for (int i = 0; i < camera.childCount; i++) {
				if (camera.GetChild (i).name == "rifle") {
					if (hitS.guns.Contains (camera.GetChild (i).gameObject)) {
						giveAmmoTo (camera.GetChild (i).gameObject, value);
						break;
					} else {
						giveAmmoTo (gun, value);
					}
				}
			}
			//GameObject rifle = .transform.
			//	hitS.guns [hitS.chosenGun];
			player.GetComponent<hit> ().CmdDespawn (netId);
		}
	}

	void giveAmmoTo (GameObject item, int amount) {
		item.GetComponent<itemInfo> ().shots += amount;
	}
}
