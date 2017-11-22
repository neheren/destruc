using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using Destruction.Standard;

public class hit : NetworkBehaviour {
	public float hitDistance;
	// Use this for initialization
	GameObject destructionState;
	destruction destruction;
	public GameObject grenate;
	float strength = 1.2f;
	public GameObject carCam;
	public GameObject playerCam;
	public List<GameObject> guns;
	public int chosenGun = 0;
	public GameObject bullet;
	public GameObject mussleFlash;
	public AudioSource gunShot;
	public AudioClip gunEmpty;
	public GameObject emitPos;
	float timeToShoot = 1f;
	bool firstFrame = true;
	public GameObject lights;
	public GameObject sentryMesh;
	public float currentGunDamage = 0;
	GameObject aim;
	bool sentryPlacement;
	public int damage;
	public bool disableMouse1;
	playerStats stats;
	gameController game;
	public int sentryAmount = 0;

	void Start () {
		game = GameObject.FindWithTag ("controller").GetComponent<gameController> ();
		stats = GetComponent<playerStats> ();
		disableMouse1 = false;
		aim = GameObject.FindGameObjectWithTag ("hud");
		chosenGun = 0;
		try {
			GameObject.Find ("Intro Camera").SetActive (false);
		} catch {
			
		}
		aim.SetActive (true);
		sentryPlacement = false;
	}

	[Client]
	public void FixedUpdate () {
		timeToShoot -= Time.fixedDeltaTime;

		if (Input.GetMouseButton (1)) {
			switch (guns [chosenGun].name) {
			case "massGun":
				if (mayShoot (0.2f)) {
					Camera eyes = this.GetComponentInChildren<Camera> ();
					Vector3 direction = (eyes.transform.forward) * 10;
					Ray ray = new Ray (eyes.gameObject.transform.position, direction);
					RaycastHit hitObj;
					Debug.DrawRay (this.gameObject.transform.position, direction, Color.red, 2f);
					if (Physics.Raycast (ray, out hitObj)) {
						if (hitObj.transform.tag == "block" && hitObj.transform.gameObject.GetComponentInParent<cubeStats> ().belongsTo == this.name) {
							NetworkInstanceId netId = hitObj.transform.gameObject.GetComponentInParent<NetworkIdentity> ().netId;
							CmdDespawn (netId);
						}
					}
				}
				break;
			}
		}

		if (Input.GetKeyDown ("e") && isClient)
			interact ();
			
		if (Input.GetKeyDown ("r") && isClient) {
			lights.GetComponent<gunController> ().reset (true);
		}
		if (Input.GetKeyDown ("f") && isClient && mayShoot (0.2f) && sentryAmount > 0) {
			sentryAmount--;
			sentryPlacement = !sentryPlacement;
			print (sentryPlacement);
		}

		if (Input.GetMouseButton (0) && !disableMouse1) {
			if (chosenGun != 0 && guns [chosenGun] != null) {
				switch (guns [chosenGun].name) {
				
				case "bazooka":
					if (mayShoot (0.5f) && tryShoot ()) {
						guns [chosenGun].GetComponent<AudioSource> ().Play ();
						shootBazooka ();
					}
					break;
				case "rifle":
					if (mayShoot (0.2f) && !lights.GetComponent<gunController> ().emptyClip && tryShoot ()) {
						lights.GetComponent<gunController> ().switchLight ();
						guns [chosenGun].GetComponent<AudioSource> ().Play ();
						shootGun ();
					}
					break;
				case "gun":
					if (mayShoot (0.17f) && tryShoot ()) {
						lights.GetComponent<gunController> ().switchLight ();
						currentGunDamage = 5;
						guns [chosenGun].GetComponent<AudioSource> ().Play ();
						shootGun ();
					}
					break;
				case "massGun":
					if (mayShoot (0.2f) && tryShoot ()) {
						//guns [chosenGun].GetComponent<AudioSource> ().Play ();
						shootMassGun ();
					}
					break;
				}
				damage = guns [chosenGun].GetComponent<itemInfo> ().damage;
			}
		}



		if (sentryPlacement) {
			chosenGun = 0;
			sentryMesh.SetActive (true);
			Camera eyes = this.GetComponentInChildren<Camera> ();
			Vector3 direction = (eyes.transform.forward) * 10;
			Ray ray = new Ray (eyes.gameObject.transform.position, direction);
			RaycastHit hitObj;
			Debug.DrawRay (this.gameObject.transform.position, direction, Color.red, 2f);
			if (Physics.Raycast (ray, out hitObj)) {
				if (hitObj.distance < 50f) {
					sentryMesh.transform.position = hitObj.point;
					sentryMesh.transform.localRotation = new Quaternion (0, 0, 0, 0);
					if (Input.GetMouseButton (0) && mayShoot (0.2f)) {
						if (hitObj.transform.gameObject.GetComponent<car> () != null) {
							CmdSpawn2 ("sentry", sentryMesh.transform.position, sentryMesh.transform.localRotation, this.name, hitObj.transform.gameObject.name);
						} else {							
							CmdSpawn2 ("sentry", sentryMesh.transform.position, sentryMesh.transform.localRotation, this.name, "");
						}
						sentryMesh.SetActive (false);
						chosenGun = 1;
						sentryPlacement = false;
					}
				}
			}
		} else {
			sentryMesh.SetActive (false);
		}
		

		for (int i = 0; i < guns.Count; i++) {
			if (Input.GetKeyDown (i.ToString ()) || firstFrame) {
				sentryPlacement = false;
				chosenGun = firstFrame ? 1 : i;
				for (int j = 1; j < guns.Count; j++) {
					guns [j].SetActive (false);
				}
				if (chosenGun != 0) {
					guns [chosenGun].SetActive (true);
					guns [chosenGun].GetComponent<animationController> ().animateGunUp ();
				}
				CmdSwitchWeapon (netId, i);

			}
		}
		firstFrame = false;
	}


	bool tryShoot () {
		itemInfo gunInfo = guns [chosenGun].GetComponent<itemInfo> ();
		if (gunInfo.shots > 0) {
			gunInfo.shots -= 1;
			return true;
		} else {
			gunShot.clip = gunEmpty;
			gunShot.Play ();
			return false;
		}
	}

	void shootMassGun () {
		Camera eyes = this.GetComponentInChildren<Camera> ();
		Vector3 direction = (eyes.transform.forward) * 10;
		Ray ray = new Ray (eyes.gameObject.transform.position, direction);
		RaycastHit hitObj;
		Debug.DrawRay (this.gameObject.transform.position, direction, Color.red, 2f);
		if (Physics.Raycast (ray, out hitObj)) {
			if (hitObj.transform.tag == "block") {
				CmdSpawn2 ("block", hitObj.transform.position + new Vector3 (0, 1, 0), hitObj.transform.rotation, this.name, "");
			} else {
				CmdSpawn2 ("block", hitObj.point, gameObject.transform.rotation, this.name, "");
			}
		}
	}

	void shootBazooka () {
		CmdSpawn2 (mussleFlash.name, emitPos.transform.position, guns [chosenGun].transform.rotation, "", "");
		Vector3 direction = (this.GetComponentInChildren<Camera> ().transform.forward) * 10;
		StartCoroutine (impact (-direction));
		guns [chosenGun].GetComponent<animationController> ().shoot ();
		CmdSpawn2 (grenate.name, GetComponentInChildren<Camera> ().transform.position + GetComponentInChildren<Camera> ().transform.forward, GetComponentInChildren<Camera> ().transform.rotation, gameObject.name, "");
		GetComponent<Rigidbody> ().AddForce (direction * 200, ForceMode.Force);
	}

	IEnumerator impact (Vector3 direction) {
		direction.Normalize ();
		for (int i = 1; i < 12; i++) {
			yield return new WaitForSeconds (Time.fixedDeltaTime);
			//transform.position += direction * 1.5f / (i * 1.5f);
			GetComponent<CharacterController> ().Move (direction * 1.5f / (i * 1.5f));
			
		}
	}

	void shootGun () {
		Camera eyes = this.GetComponentInChildren<Camera> ();
		Vector3 direction = (eyes.transform.forward) * 10;
		CmdSpawn2 (bullet.name, guns [chosenGun].transform.position, guns [chosenGun].transform.rotation, "", "");
		CmdSpawn2 (mussleFlash.name, emitPos.transform.position, guns [chosenGun].transform.rotation, "", "");
		Ray ray = new Ray (eyes.gameObject.transform.position, direction);
		RaycastHit hitObj;

		guns [chosenGun].GetComponent<animationController> ().shoot ();

		if (Physics.Raycast (ray, out hitObj)) {
			if (hitObj.distance < hitDistance) {
				CmdSpawn2 ("dust", hitObj.point, Quaternion.Euler (0, 0, 0), "", "");

				//HIT GROUND
				if (hitObj.transform.gameObject.name == "plane50" || hitObj.transform.gameObject.GetComponent<newPerlin> () != null) {
					newPerlin perlinScript = hitObj.transform.gameObject.GetComponent <newPerlin> ();
					Debug.DrawRay (this.gameObject.transform.position, direction, Color.red, 2f);

					Mesh mesh = hitObj.collider.gameObject.GetComponent<MeshFilter> ().mesh;
					Vector3[] tris = mesh.vertices;
					int[] triangles = mesh.triangles;
					CmdDestrucion (hitObj.transform.gameObject.name, triangles [hitObj.triangleIndex * 3 + 0], strength);
					CmdDestrucion (hitObj.transform.gameObject.name, triangles [hitObj.triangleIndex * 3 + 1], strength);
					CmdDestrucion (hitObj.transform.gameObject.name, triangles [hitObj.triangleIndex * 3 + 2], strength);
					mesh.vertices = tris;
					mesh.RecalculateNormals ();
					perlinScript.updateVertices = true;
				

					//HIT PLAYER
				} else if (hitObj.transform.gameObject.GetComponent<playerStats> () != null) {
					if (hitObj.transform.name != gameObject.name) {
						playerStats hitOnPlayer = hitObj.transform.gameObject.GetComponent<playerStats> ();
						if (gameController.isDead (hitOnPlayer.health, (int)currentGunDamage)) {
							print ("got em");
							CmdGetControllerAndMessage (hitOnPlayer.name, stats.name, "gun");

						}
						CmdDamagePlayer (hitOnPlayer.netId, damage);
						CmdSpawn2 ("blod", hitObj.point, Quaternion.Euler (-hitObj.point), "", "");
					}

					//HIT CUBE
				} else if (hitObj.transform.gameObject.tag == "block") {
					cubeStats hitOnCube = hitObj.transform.gameObject.GetComponentInParent<cubeStats> ();
					CmdDamageCube (hitOnCube.netId, damage);
					CmdSpawn2 ("blod", hitObj.point, Quaternion.Euler (-hitObj.point), "", "");	

					//HIT CAR
				} else if (hitObj.transform.gameObject.GetComponent<car> () != null) {
					CmdDamageCar (hitObj.transform.gameObject.GetComponent<car> ().netId, damage);
					
					//HIT SENTRYGUN
				} else if (hitObj.transform.gameObject.GetComponent<sentry> () != null) {
					CmdDamageSentry (hitObj.transform.gameObject.GetComponent<sentry> ().netId, damage);
				} else if (hitObj.transform.gameObject.tag == "glass") {
					hitObj.transform.gameObject.GetComponent<Trigger> ().TriggerDestruction (hitObj.point, 1f);
				}
			}
		}
	}

	void interact () {
		Camera eyes = this.GetComponentInChildren<Camera> ();
		Vector3 direction = (eyes.transform.forward) * 10;

		Ray ray = new Ray (eyes.gameObject.transform.position, direction);
		RaycastHit hitObj;
		Debug.DrawRay (eyes.gameObject.transform.position, direction, Color.red, 2f);

		if (Physics.Raycast (ray, out hitObj, 4f)) {
			print (hitObj.transform.gameObject.name);
			if (hitObj.transform.gameObject.GetComponent<car> () != null) {
				car carScript = hitObj.transform.gameObject.GetComponent<car> ();
				CmdAssignAuthority (gameObject.name, hitObj.transform.transform.gameObject.name);
				carScript.player = this.gameObject;
				carScript.team = GetComponent<playerStats> ().team;
				Camera carCamera = hitObj.transform.gameObject.GetComponentInChildren<Camera> (true);
				carCam = carCamera.gameObject;
				carScript.switchCamToCar (true, gameObject.transform.position);
			}
			if (hitObj.transform.gameObject.tag == "door") {
				hitObj.transform.gameObject.GetComponentInParent<doorController> ().toggleDoor ();
			}
			if (hitObj.transform.gameObject.tag == "buyZone") {
				hitObj.transform.parent.transform.gameObject.GetComponent<buyZone> ().openBuyMenu (this.gameObject);
			}
			print (hitObj.transform.gameObject.tag);
		}
	}

	public bool mayShoot (float reloadTime) {
		if (timeToShoot < 0) {
			timeToShoot = reloadTime;
			return true;
		} else {
			return false;
		}
	}


	////////////////////////////////
	//NETWORKING....
	////////////////////////////////


	[Command]
	public void CmdSpawn2 (string obj, Vector3 pos, Quaternion rotation, string spawnedBy, string nameOfParent) {
		GameObject objectToBeSpawned = (GameObject)Resources.Load (obj, typeof(GameObject));

		if (objectToBeSpawned.name == "grenate") {
			objectToBeSpawned.GetComponent<granate> ().spawnedBy = spawnedBy;
		}
		if (objectToBeSpawned.name == "block") {
			objectToBeSpawned.GetComponent<cubeStats> ().belongsTo = spawnedBy;
		}

		if (objectToBeSpawned.name == "sentry") {
			sentry sentryScript = objectToBeSpawned.GetComponent<sentry> ();
			objectToBeSpawned.GetComponent<sentry> ().belongsTo = spawnedBy;
			sentryScript.team = GetComponent<playerStats> ().team;
			if (nameOfParent != "")
				sentryScript.parentUnder = GameObject.Find (nameOfParent).transform;
		}
			

		GameObject go = (GameObject)Instantiate (objectToBeSpawned, pos, rotation);
		NetworkServer.Spawn (go);
		if (objectToBeSpawned.name == "sentry") {
			sentry sentryScript = objectToBeSpawned.GetComponent<sentry> ();
			sentryScript.belongsTo = "";
			sentryScript.parentUnder = null;
			sentryScript.team = 0;
		}


	}



	[Command]
	public void CmdAssignAuthority (string playerName, string carName) {
		GameObject player = GameObject.Find (playerName);
		GameObject car = GameObject.Find (carName);
		NetworkConnection con;
		try {
			con = player.GetComponent<NetworkIdentity> ().connectionToClient;
		} catch {
			con = null;
			print ("Error, no connection to client");
		}
		NetworkIdentity carNetId = car.GetComponent<NetworkIdentity> ();
		if (carNetId.clientAuthorityOwner != null) {
			carNetId.RemoveClientAuthority (carNetId.clientAuthorityOwner);
		}
		carNetId.AssignClientAuthority (con);
		print (GetComponent<NetworkTransform> ().localPlayerAuthority);

	}


	[Command]
	public void CmdGetControllerAndMessage (string killedPlayerId, string killedById, string with) {
		gameController game = GameObject.FindWithTag ("controller").GetComponent<gameController> ();
		game.CmdAnounceDeath (killedPlayerId, killedById, with);
	}


	[Command]
	public void CmdDespawn (NetworkInstanceId netId) {
		Destroy (NetworkServer.FindLocalObject (netId));
	}



	[Command]
	public void CmdDamageCube (NetworkInstanceId netId, int h) {
		string name = gameController.CUBE + netId;
		RpcDamageCube (name, h);
	}

	[ClientRpc]
	public void RpcDamageCube (string name, int h) {
		print (name);
		print (h);
		GameObject cube = GameObject.Find (name);
		print (cube);
		cube.GetComponent<cubeStats> ().setHealth (h);
	}



	[Command]
	public void CmdDamageCar (NetworkInstanceId netId, int h) {
		string name = gameController.CAR + netId;
		RpcDamageCar (name, h);
	}

	[ClientRpc]
	public void RpcDamageCar (string name, int h) {
		GameObject car = GameObject.Find (name);
		car.GetComponent<car> ().health -= h;
	}


	[Command]
	public void CmdDamageSentry (NetworkInstanceId netId, int h) {
		string name = gameController.SENTRY + netId;
		RpcDamageSentry (name, h);
	}

	[ClientRpc]
	public void RpcDamageSentry (string name, int h) {
		GameObject sentry = GameObject.Find (name);
		sentry.GetComponent<sentry> ().health -= h;
	}



	[Command]
	public void CmdDestrucion (string planeName, int verticeIndex, float strenght) {
		RpcDestroyGround (planeName, verticeIndex, strenght);
	}

	[ClientRpc]
	public void RpcDestroyGround (string planeName, int verticeIndex, float strenght) {
		GameObject.Find (planeName).GetComponent<newPerlin> ().destroyGround (verticeIndex, strenght);
	}



	[Command]
	public void CmdDamagePlayer (NetworkInstanceId netId, int h) {
		string name = gameController.PLAYER + netId;
		RpcDamagePlayer (name, h);
	}

	[ClientRpc]
	public void RpcDamagePlayer (string name, int h) {
		GameObject player = GameObject.Find (name);
		player.GetComponent<playerStats> ().damagePlayer (h);
	}


	[Command]
	public void CmdSwitchWeapon (NetworkInstanceId netId, int weapon) {
		string name = gameController.PLAYER + netId;
		RpcSwitchWeapon (name, weapon);
	}

	[ClientRpc]
	public void RpcSwitchWeapon (string name, int weapon) {
		GameObject player = GameObject.Find (name);
		if (!isLocalPlayer) { // WTF. HVORFOR VIRKER DET????
			Transform playersFps = player.transform.GetChild (0);
			int gunAmount = playersFps.childCount;
			for (int i = 0; i < gunAmount; i++) {
				if (i == weapon) {
					playersFps.GetChild (i).gameObject.SetActive (true);
				} else
					playersFps.GetChild (i).gameObject.SetActive (false);

			}
		}
	}
		
}
