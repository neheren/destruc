using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyOnCollide : MonoBehaviour {
	public float started;
	// Use this for initialization
	void Start () {
		started = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col) {
		if (col.transform.name == "Cube") {
			if (started > col.gameObject.GetComponent<destroyOnCollide> ().started) {
				Destroy (this.transform.parent.gameObject);
			}
		}
	}
}
