using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour {
	public GameObject hinge;
	bool closed = true;

	Animation animation;

	// Use this for initialization
	void Start () {
		animation = hinge.GetComponent<Animation> ();
	}


	public void openDoor () {
		animation.PlayQueued ("openDoor");
		closed = false;
	}

	public void closeDoor () {
		animation.PlayQueued ("closeDoor");
		closed = true;
	}

	public void toggleDoor () {
		if (closed)
			openDoor ();
		else
			closeDoor ();
	}

}
