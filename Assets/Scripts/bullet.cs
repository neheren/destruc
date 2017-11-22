using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {
	Vector3 bulletDirection;
	float airTime;
	public float speed;
	public float despawnTime;
	public bool moveSideways;
	// Use this for initialization
	void Start () {
		if (moveSideways) {
			bulletDirection = transform.right * -speed;
		} else {
			bulletDirection = transform.forward * speed;
			
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		airTime += Time.fixedDeltaTime;
		if (moveSideways) {
			gameObject.transform.localPosition += bulletDirection;
		} else {
			gameObject.transform.localPosition += bulletDirection;

		}
		if (airTime > despawnTime)
			Destroy (this.gameObject);
	}
}
