using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
}

public class car : NetworkBehaviour {
	public GameObject fire;
	public int health = 300;
	public float exitTime;
	public List<AxleInfo> axleInfos;
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public GameObject carCam;
	public GameObject player;
	public GameObject explosion;
	public bool playerInCar = false;
	public AudioSource startUp;
	public AudioSource running;
	bool didRunEnter = false;
	public GameObject mainBody;
	public GameObject roof;
	hit playerScript;
	public GameObject[] exhausts;
	public GameObject destroyedCar;
	public int team;
	public Material roofTeam1;
	public Material roofTeam2;
	public Material team1;
	public Material team2;
	public Vector3 frontSeat;

	void Start () {
		frontSeat = new Vector3 (-1, 0, 0);
		this.name = gameController.CAR + netId;
		exitTime = 0.5f;
	}

	public void setColor () {
		Material chosenColor = (team == 1) ? team1 : team2;
		Material chosenRoofColor = (team == 1) ? roofTeam1 : roofTeam2;
		roof.GetComponent<MeshRenderer> ().material = chosenRoofColor;
		mainBody.GetComponent<MeshRenderer> ().material = chosenColor;
	}

	public void ApplyLocalPositionToVisuals (WheelCollider collider) {
		if (collider.transform.childCount == 0) {
			return;
		}
     
		Transform visualWheel = collider.transform.GetChild (0);
     
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose (out position, out rotation);
     
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}

	bool spawnOnce = true;
	Vector3 playerPosOffset = new Vector3 (4f, 0f, 0f);

	public void LateUpdate () {
		if (player != null)
			player.transform.position = transform.position - transform.right / 2;
	}

	public void FixedUpdate () {

		if (health < 80 && spawnOnce) {
			spawnOnce = false;
			GameObject fireInmotor = Instantiate (fire, transform.position, Quaternion.Euler (0, 0, 0));
			fireInmotor.transform.parent = this.transform;
		} else if (health <= 0) {
			//player is dead too:
			if (player != null) {
				player.GetComponent<playerStats> ().damagePlayer (100);
				//player.GetComponent<hit>().CmdGetControllerAndMessage(player.name, )
				exitCar ();
			}

			//car explosion:
			Instantiate (destroyedCar, this.transform.position, Quaternion.Euler (0, 0, 0));
			Instantiate (explosion, this.transform.position, Quaternion.Euler (0, 0, 0));

			Destroy (this.gameObject);
			for (int i = 0; i < gameObject.transform.childCount; i++) {
				if (gameObject.transform.GetChild (i).name != "bodyParent")
					Destroy (gameObject.transform.GetChild (i));
			}

		}

		bool runOnExit = false;
		if (carCam.activeInHierarchy && playerInCar && isClient) {
			if (didRunEnter == false) {
				runOnEnter ();
				didRunEnter = true;
			}
			exitTime -= Time.deltaTime;

			
			float motor = maxMotorTorque * Input.GetAxis ("Vertical");
			float steering = maxSteeringAngle * Input.GetAxis ("Horizontal");
			drive (steering, motor);

			foreach (AxleInfo axleInfo in axleInfos) {
				ApplyLocalPositionToVisuals (axleInfo.leftWheel);
				ApplyLocalPositionToVisuals (axleInfo.rightWheel);
			}
			running.volume = GetComponent<Rigidbody> ().velocity.magnitude / 50;
			runOnExit = true;
			if (Input.GetKeyDown ("e") && mayExit ())
				exitCar ();
		}

            
	}

	void exitCar () {
		player.transform.position = transform.position + playerPosOffset;
		running.Stop ();
		didRunEnter = false;
		playerScript = player.GetComponent<hit> ();
		playerScript.enabled = true;
		switchCamToCar (false, this.gameObject.transform.position + playerPosOffset);
		player = null;

		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.motor) {
				axleInfo.leftWheel.motorTorque = 0f;
				axleInfo.rightWheel.motorTorque = 0f;
			}
		}
	}


	// not to be run internally
	public void switchCamToCar (bool switchIt, Vector3 pos) {
		player.GetComponent<hit> ().enabled = !switchIt;
		player.GetComponent<CharacterController> ().enabled = !switchIt;
		player.GetComponent<MeshCollider> ().enabled = !switchIt;
		player.GetComponent<MeshRenderer> ().enabled = switchIt;
		player.GetComponent<AudioSource> ().enabled = !switchIt;
		player.transform.GetChild (1).gameObject.SetActive (!switchIt);

		//gameObject.SetActive (!switchIt);
		player.transform.position = pos;
		carCam.SetActive (switchIt);
		playerInCar = switchIt;
	}


	public bool mayExit () {
		if (exitTime < 0) {
			exitTime = 0.5f;
			return true;
		} else {
			return false;
		}
	}

	void drive (float steering, float motor) {
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if (axleInfo.motor) {
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			}
		}
		float standardExhaust = !playerInCar ? 0 : 20;
		foreach (var exhaust in exhausts) {
			ParticleSystem.EmissionModule em = exhaust.GetComponent<ParticleSystem> ().emission;
			em.rateOverTime = motor / 1000f + standardExhaust;
		}
	}

	void runOnEnter () {
		setColor ();
		carCam.SetActive (true);
		startUp.Play ();
		running.Play ();
	}
}