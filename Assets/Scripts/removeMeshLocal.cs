using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class removeMeshLocal : NetworkBehaviour
{

	// Use this for initialization
	void Start ()
	{
		if (isLocalPlayer) {
			gameObject.GetComponent<MeshRenderer> ().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
