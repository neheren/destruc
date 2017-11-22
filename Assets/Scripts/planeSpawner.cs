using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeSpawner : MonoBehaviour
{
	public GameObject plane;
	public int amountX = 5;
	public int amountY = 5;
	public float planeMargin = 2.1f;

	// Use this for initialization
	void Start ()
	{
		float scale = plane.transform.lossyScale.x / 100;
		for (int x = 0; x < amountX; x++) {
			for (int y = 0; y < amountY; y++) {
				GameObject spawnedPlane = Instantiate (plane, gameObject.transform.position + new Vector3 (x * (scale + planeMargin), 0f, y * (scale + planeMargin)), new Quaternion ());
				spawnedPlane.name = "plane " + x + "+" + y;
				spawnedPlane.transform.parent = gameObject.transform;
			}
		}
	}

}
