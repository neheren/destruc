using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyAfterSecs : MonoBehaviour {

	float time;
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > 4f)
			Destroy (this.gameObject);
	}
}
