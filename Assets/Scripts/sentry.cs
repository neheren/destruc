using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class sentry : NetworkBehaviour {

	public GameObject emissionPoint;
	public GameObject emissionFire;
	public GameObject explosion;
	public float health = 500;

	public float damageEach = 0.1f;
	public int damage = 8;
	float time;
	public string belongsTo;
	public Transform parentUnder;
	public int team;
	gameController game;

	public Material team1;
	public Material team2;
	public GameObject body;

	AudioSource shotSound;
	// Use this for initialization
	void Start () {
		Material chosenColor = (team == 1) ? team1 : team2;
		body.GetComponent<MeshRenderer> ().material = chosenColor;
		time = Random.Range (0, damageEach);
		if (parentUnder != null)
			this.transform.parent = parentUnder;
		this.name = gameController.SENTRY + netId;
		shotSound = GetComponent<AudioSource> ();
		game = GameObject.FindWithTag ("controller").GetComponent<gameController> ();
	}
	
	// Update is called once per frame
	public void setHealth (int h) {
		health -= h;
	}

	void FixedUpdate () {
		if (health <= 0) {
			Instantiate (explosion, transform.position, emissionPoint.transform.rotation);
			GameObject.Destroy (this.gameObject);
			this.gameObject.SetActive (false);
		}
		time += Time.fixedDeltaTime;

	}

	void OnTriggerStay (Collider col) {
		if (time > damageEach) {
			int colTeam = 0;
			colTeam = (col.gameObject.GetComponent<car> () != null) ? col.gameObject.GetComponent<car> ().team : 0;
			colTeam = (col.gameObject.GetComponent<hit> () != null) ? col.gameObject.GetComponent<playerStats> ().team : 0;
			if (team != colTeam && (col.gameObject.GetComponent<hit> () != null || col.gameObject.GetComponent<car> () != null)) {
				time = 0;
				Ray ray = new Ray (emissionPoint.transform.position, col.transform.position - emissionPoint.transform.position);
				Debug.DrawRay (emissionPoint.transform.position, col.transform.position - emissionPoint.transform.position, Color.blue, 2f);
				RaycastHit hitObj;
				if (Physics.Raycast (ray, out hitObj)) {
					if (team != colTeam) {
						if ((col.transform.position - this.transform.position).x > 0)
							this.transform.rotation = Quaternion.Euler (0, Vector3.Angle (new Vector3 (0, 0, 1), col.transform.position - this.transform.position), 0);
						else
							this.transform.rotation = Quaternion.Euler (0, -Vector3.Angle (new Vector3 (0, 0, 1), col.transform.position - this.transform.position), 0);

					}
					if (hitObj.transform.gameObject.GetComponent<playerStats> () != null) {

						playerStats playerScript = hitObj.transform.gameObject.GetComponent<playerStats> ();
						if (team != playerScript.team) {
							playerScript.damagePlayer (damage);
							if (playerScript.health <= 0 && !playerScript.dead) {
								playerScript.dead = true;
								game.CmdAnounceDeath (playerScript.name, belongsTo, "Sentry");
							}
							shoot (col.transform.position);
						}
					}
					if (hitObj.transform.gameObject.GetComponent<car> () != null) {
						car carScript = hitObj.transform.gameObject.GetComponent<car> ();
						if (team != carScript.team) {
							carScript.health -= damage;
							shoot (col.transform.position);
						}
					}
				}
			}
		}
	}


	void shoot (Vector3 pos) {
		shotSound.Play ();
		Instantiate (emissionFire, emissionPoint.transform.position, emissionPoint.transform.rotation);
		Debug.DrawRay (this.gameObject.transform.position, pos - transform.position, Color.red, 2f);
	}



}
