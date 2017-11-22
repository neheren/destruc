using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class cubeStats : NetworkBehaviour {

	public int health = 500;
	public string belongsTo;
	int startHealth;

	void Start () {
		startHealth = health;
		this.name = gameController.CUBE + netId;
	}

	public void setHealth (int h) {
		health -= h;
		UpdateCube ();
	}

	public void UpdateCube () {
		if (health <= 0) {
			Destroy (gameObject); 
		}
		transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetColor ("_OutlineColor", new Color (1, health / (float)startHealth, health / (float)startHealth));
	}
		
}
