using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Destruction.Standard;

public class granate : NetworkBehaviour {

	newPerlin perlinScript;
	Mesh mesh;
	Vector3[] verts;
	int[] triangles;

	public GameObject explosion;
	public int playerDamage = 4;
	public int amount = 12;
	public int radius = 5;
	public float explosionHeight = 2.5f;
	public float strenght = 2f;
	public string spawnedBy;
	float destroyBuildingEvery = 0f;
	gameController game;

	void Start () {
		game = GameObject.FindWithTag ("controller").GetComponent<gameController> ();
	}

	void OnTriggerEnter (Collider collision) {
		if (!collision.isTrigger && collision.transform.gameObject.name != spawnedBy) {
			if (collision.transform.gameObject.GetComponent<newPerlin> () != null) {
				mesh = collision.gameObject.GetComponent<MeshFilter> ().mesh;
				perlinScript = collision.gameObject.GetComponent <newPerlin> ();
				
				verts = mesh.vertices;
				triangles = mesh.triangles;
				perlinScript.updateVertices = true;
			}
			
			explode ();
			Instantiate (explosion, transform.position, Quaternion.Euler (0, 0, 0));
			Destroy (gameObject);
		}
	}


	void explode () {
		for (float x = 0; x < 360; x += amount) {
			for (float y = 0; y < 360; y += amount) {
				burst (new Vector3 (Mathf.Sin (Mathf.Deg2Rad * y) * radius, Mathf.Cos (Mathf.Deg2Rad * y) * Mathf.Sin (Mathf.Deg2Rad * x) * radius, Mathf.Cos (Mathf.Deg2Rad * x) * radius), radius + 1, strenght);
			}
		}
	}


	void burst (Vector3 direction, float hitDistance, float strenght) {
		Ray ray = new Ray (gameObject.transform.position + new Vector3 (0, explosionHeight, 0), direction);
		RaycastHit hitObj;

		//Debug.DrawRay (this.gameObject.transform.position, direction, Color.blue, 10f);

		if (Physics.Raycast (ray, out hitObj, 5f)) {
			if (hitObj.transform.gameObject.GetComponent<newPerlin> () != null) {
				perlinScript = hitObj.transform.gameObject.GetComponent <newPerlin> ();

				try {
					int triangle = triangles [hitObj.triangleIndex * 3 + 0];
					
					perlinScript.destroyGround (triangle, strenght);
					perlinScript.destroyGround (triangles [hitObj.triangleIndex * 3 + 1], strenght);
					perlinScript.destroyGround (triangles [hitObj.triangleIndex * 3 + 2], strenght);
					
				} catch {
					//print ("Error in getting triangle index");
				}

			} else if (destroyBuildingEvery % 1 == 0 && hitObj.transform.gameObject.GetComponent<Trigger> () != null) {
				hitObj.transform.gameObject.GetComponent<Trigger> ().TriggerDestruction (hitObj.point, 0.1f);
				//	Debug.DrawRay (this.gameObject.transform.position, direction, Color.red, 10f);
			
			} else if (hitObj.transform.gameObject.GetComponent<playerStats> () != null) {
				playerStats player = hitObj.transform.gameObject.GetComponent<playerStats> ();
				if (gameController.isDead (player.health, playerDamage)) {
					game.CmdAnounceDeath (player.name, spawnedBy, "bazooka");
				}
				player.damagePlayer (playerDamage);
			} else if (hitObj.transform.gameObject.tag == "block") {
				cubeStats cube = hitObj.transform.gameObject.GetComponentInParent<cubeStats> ();
				cube.setHealth (playerDamage);
			} else if (hitObj.transform.gameObject.GetComponent<car> () != null) {
				hitObj.transform.gameObject.GetComponent<car> ().health -= 1;
			} else if (hitObj.transform.gameObject.GetComponent<sentry> () != null && hitObj.collider.gameObject.name != "shield") {
				hitObj.transform.gameObject.GetComponent<sentry> ().health -= 1;
				destroyBuildingEvery += 0.5f;
			}
		}
		

	}
}
