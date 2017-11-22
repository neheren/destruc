using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steadyCam : MonoBehaviour
{
	public GameObject car;
	Vector3 startingCamPos;
	// Use this for initialization
	void Start ()
	{
		startingCamPos = new Vector3 (0f, gameObject.transform.position.y, -gameObject.transform.position.z * 2);
	}
	
	// Update is called once per frame
	void Update ()
	{
		gameObject.transform.position = startingCamPos + car.transform.position;

		float yRotation = car.transform.localRotation.x;
		Quaternion cameraRot = new Quaternion (0, car.transform.localRotation.y, 0, car.transform.localRotation.w);
		gameObject.transform.SetPositionAndRotation (startingCamPos + car.transform.position, cameraRot);
		gameObject.transform.LookAt (car.transform);
	}
}
