using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class disabler : NetworkBehaviour {
	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	Camera sceneCam;
	private NetworkManager manager;

	private GameObject introMenu;

	void Start () {
		if (!isLocalPlayer) {
			DisableComponents ();
		}
	}

	void DisableComponents () {
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false; 
		}
	}
}