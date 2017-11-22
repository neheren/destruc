using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skrtLogic : MonoBehaviour {

	float checkEvery = 0.3f;
	float time = 0;
	float previousVelocity = 0;
	bool shouldSkrt;
	bool didSkrt;
	public AudioSource skrt;
	public car carScript;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > checkEvery) {
			if (carScript.playerInCar && (Input.GetKey ("s") || Input.GetKey ("s") || Input.GetKey ("a") || Input.GetKey ("d"))) {
				time = 0;
				float currentVelocity = GetComponent<Rigidbody> ().velocity.magnitude;
				if (previousVelocity > currentVelocity + 1f) {
					shouldSkrt = true;
				} else {
					shouldSkrt = false;
					didSkrt = false;
				}

				previousVelocity = currentVelocity;

			} else {
				shouldSkrt = false;
				didSkrt = false;
			}
		}

		if (shouldSkrt && !didSkrt && !skrt.isPlaying) {
			skrt.Play ();
			didSkrt = true;
		}
		if (!shouldSkrt) {
			skrt.Stop ();
		} 
		
	}
}
