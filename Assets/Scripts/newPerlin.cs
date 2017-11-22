using UnityEngine;
using System;
using System.Collections.Generic;

//using UnityEngine.Networking;
using System.Collections;
using System.Linq;


public class newPerlin : MonoBehaviour {
	public float power = 3.0f;
	public float scale = 1.0f;
	private Vector2 v2SampleStart = new Vector2 (0f, 0f);
	float scalar;

	public struct destructionPoint {
		public Vector3 destruc;

		public destructionPoint (Vector3 _destruc) {
			destruc = _destruc;
		}
	}

	public class syncDestruc : List<destructionPoint> {
		
	}

	public syncDestruc destructionList = new syncDestruc ();


	public Vector3[] destruction;
	public bool updateVertices = true;
	public bool allwaysDraw;


	void Awake () {
		int vertixesLength = GetComponent<MeshFilter> ().sharedMesh.vertexCount;
		for (int i = 0; i < vertixesLength; i++) {
			destructionList.Add (new destructionPoint (new Vector3 ()));
		}
		scalar = gameObject.transform.lossyScale.x;
		v2SampleStart.x = this.gameObject.transform.position.x / scalar * scale * 100;
		v2SampleStart.y = this.gameObject.transform.position.z / scalar * scale * 100;
		MakeSomeNoise ();
		updateVertices = true;
	}

	void Update () {
		v2SampleStart.x = this.gameObject.transform.position.x / scalar * scale * 100;
		v2SampleStart.y = this.gameObject.transform.position.z / scalar * scale * 100;
		if (updateVertices || allwaysDraw)
			MakeSomeNoise ();
	}


	public void destroyGround (int verticeIndex, float strenght) {
		float depth = destructionList [verticeIndex].destruc.y - 0.01f;
		float groundHardness = 0.0001f * (strenght * 2) / -depth;
		Vector3 newDestruction = destructionList [verticeIndex].destruc - new Vector3 (0, groundHardness * strenght, 0);
		destructionList [verticeIndex] = new destructionPoint (newDestruction);
		updateVertices = true;
	}

	void MakeSomeNoise () {
		MeshFilter mf = GetComponent<MeshFilter> ();
		MeshCollider mc = GetComponent<MeshCollider> ();
		Vector3[] vertices = mf.mesh.vertices;
		for (int i = 0; i < vertices.Length; i++) { 
			float xCoord = v2SampleStart.x + vertices [i].x * scale * 100;
			float yCoord = v2SampleStart.y + vertices [i].z * scale * 100;


			float p1 = (Mathf.PerlinNoise (xCoord, yCoord) - 0.5f) * power / 100;
			float p2 = (Mathf.PerlinNoise (xCoord / 3, yCoord / 3) - 0.5f) * (power * 2) / 100;
			float p3 = Mathf.Sin (Mathf.PerlinNoise (xCoord / 10, yCoord / 10)) * 2;
			vertices [i].y = (p1 + p2) * Mathf.Log (p3) * 3 + destructionList [i].destruc.y;
			if (vertices [i].x < 0.005f && vertices [i].z < 0.005f) {
			} else {
				vertices [i].y = (p1 + p2) * Mathf.Log (p3) * 3;
			}
			if (vertices [i].x > -0.005f && vertices [i].z > -0.005f) {
			} else {
				vertices [i].y = (p1 + p2) * Mathf.Log (p3) * 3;
			}
		}

		mf.mesh.SetVertices (vertices.ToList ());
		mf.sharedMesh = mf.mesh; 
		mf.mesh.RecalculateNormals ();
		mf.mesh.RecalculateBounds ();
		mc.sharedMesh = mf.mesh; //different!!
		updateVertices = false;
	}
} 