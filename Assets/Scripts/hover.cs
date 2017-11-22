using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hover : MonoBehaviour {
	float t;
	Vector3 originalTransform;
	public float speed = 100f;
	public float amount = 0.5f;
	// Use this for initialization
	void Start () {
		originalTransform = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		t++;
		transform.position = originalTransform + new Vector3 (0, Mathf.Sin (t / speed) * amount, 0);
	}
}
