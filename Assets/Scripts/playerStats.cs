using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Networking;

public class playerStats : NetworkBehaviour {
	public int health = 100;
	[SyncVar]
	public int kills = 0;
	[SyncVar]
	public int deaths = 0;

	public int money = 1000;
	float collisionDamageMultiplier = 1.5f;
	float collisionVelocityFilter = 5f;
	public GUIStyle style;
	public GUIStyle styleShadow;
	public GUIStyle anounceStyle;
	public GUIStyle anounceStyle2;
	public GUIStyle deathScreenStyle;
	public int team;
	public string playerName;
	public bool gameIsRunning;
	public string flagCatured = "";
	public bool dead;
	gameController game;
	hit hitScript;
	public Material team1Color;
	public Material team2Color;
	public string scoreBoard;
	bool deathScreen = false;
	public Texture damageTex;
	public Texture deathScreenTex;
	public Animation damageAnimation;
	// Use this for initialization

	void Start () {
		hitScript = GetComponent<hit> ();
		dead = false;
		game = GameObject.FindWithTag ("controller").GetComponent<gameController> ();
		this.name = gameController.PLAYER + netId;
		gameIsRunning = false;
		playerName = (gameController.playerName != "" || gameController.playerName == "ENTER NAME") ? gameController.playerName : this.name;
		team = 0;
		GetComponent<FirstPersonController> ().m_MouseLook.lockCursor = false;
	}

	// Update is called once per frame
	void Update () {
		if (true || isLocalPlayer) { // ????
			if (health <= 0) {
				StartCoroutine (resetPosition (true));
			}
		}
	}

	public void damagePlayer (int damage) {
		health -= damage;
		damageAnimation.Stop ();
		damageAnimation.Play ();
	}

	void OnCollisionEnter (Collision col) {
		float speed = Vector3.Magnitude (col.relativeVelocity);
		if (speed < collisionVelocityFilter) {
			if (health <= 0) {
				damagePlayer ((int)(speed * collisionDamageMultiplier));
				game.CmdAnounceDeath (null, null, "Velocity");
				StartCoroutine (resetPosition (true));
			}
		}
	}

	Vector2 pos = new Vector2 (Screen.width - 70, Screen.height - 30);
	Vector2 size = new Vector2 (300, 20);
	float sinVar = 0;

	void OnGUI () {
		sinVar += 1;
		if (!gameIsRunning) {
			if (isServer && isLocalPlayer) {
				if (whoPickedTeams ()) {
					if (GUI.Button (new Rect (Screen.width / 2 - 75, 10, 150, 25), "Start game")) {
						CmdStartGame ();
					}
				}
			}

			if (isLocalPlayer) {
				GUI.Box (new Rect (Screen.width / 2 - 75, 50, 150, 25), "Pick team");
				if (GUI.Button (new Rect (Screen.width / 2 - 75, 85, 70, 20), "Team 1")) {
					team = 1;
					CmdUpdateTeam (netId, team);
				}
				if (GUI.Button (new Rect (Screen.width / 2 + 5, 85, 70, 20), "Team 2")) {
					team = 2;
					CmdUpdateTeam (netId, team);
				}


			}

		} else if (isLocalPlayer) {
			if (deathScreen) {
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), deathScreenTex, ScaleMode.ScaleAndCrop, true);
				deathScreenStyle.normal.textColor = Color.red;
				GUI.Box (new Rect (0, 0, Screen.width, Screen.height), "You died.", deathScreenStyle);
				deathScreenStyle.normal.textColor = Color.white;
				GUI.Box (new Rect (1, -1, Screen.width, Screen.height), "You died.", deathScreenStyle);
			}

			if (game.killFeedString != "") {
				anounceStyle.normal.textColor = new Color (0.5f, 0.1f, 0);
				GUI.Box (new Rect (Screen.width / 2 - 1, 1, Screen.width / 2, 100), game.killFeedString, anounceStyle);
				anounceStyle.normal.textColor = Color.white;
				GUI.Box (new Rect (Screen.width / 2, 0, Screen.width / 2, 100), game.killFeedString, anounceStyle);
			}
			if (game.anouncement != "") {
				float sin = (1 + Mathf.Sin (sinVar / 10)) / 2;
				anounceStyle2.normal.textColor = new Color (sin - 0.1f, sin / 3, 0);
				GUI.Box (new Rect (0 - 2, 2, Screen.width, Screen.height), game.anouncement, anounceStyle2);
				anounceStyle2.normal.textColor = Color.white;
				GUI.Box (new Rect (0, 0, Screen.width, Screen.height), game.anouncement, anounceStyle2);
			}

			if (Input.GetKey (KeyCode.Tab)) {
				calcScoreboard ();
				GUI.Box (new Rect (Screen.width / 4, 50f, Screen.width / 2, 300f), scoreBoard);
			}

			GUI.BeginGroup (new Rect (pos.x - 30, pos.y, size.x, size.y));
			style.normal.textColor = new Color (1 - (float)health / 100, 0, 0);
			GUI.Label (new Rect (-1, 1, size.x, size.y), "HP: " + health.ToString (), style);
			style.normal.textColor = Color.white;
			GUI.Label (new Rect (0, 0, size.x, size.y), "HP: " + health.ToString (), style);
			GUI.EndGroup ();
				

			GUI.BeginGroup (new Rect (-20, pos.y, size.x, size.y));
			string gunShots = "";
			if (hitScript.chosenGun != 0) {
				gunShots = hitScript.guns [hitScript.chosenGun].GetComponent<itemInfo> ().shots.ToString ();
			}
			string text = "Round " + game.round + "   |   " + money.ToString () + "$" + "   |          -   " + gunShots;
			style.normal.textColor = Color.black;
			GUI.Label (new Rect (-1, 1, size.x, size.y), text, style);
			style.normal.textColor = Color.white;
			GUI.Label (new Rect (0, 0, size.x, size.y), text, style);
			GUI.EndGroup ();

		}
	}

	void calcScoreboard () {
		scoreBoard = "Round " + gameController.getController ().round + "\n \n";
		string team1 = "";
		string team2 = "";
		GameObject[] players = GameObject.FindGameObjectsWithTag ("player");
		foreach (var player in players) {
			playerStats stats = player.GetComponent<playerStats> ();
			if (stats.team == 1) {
				team1 += stats.playerName + " - " + stats.kills + " / " + stats.deaths;
			} else if (stats.team == 2) {
				team2 += stats.playerName + " - " + stats.kills + " / " + stats.deaths;
			} else {
			}
		}
		scoreBoard += "Team 1\n" + ((team1 != "") ? team1 : "-") + "\n \n Team 2\n " + ((team2 != "") ? team2 : "-");
	}

	public IEnumerator resetPosition (bool died) {
		if (died) {
			deathScreen = true;
			yield return new WaitForSeconds (3f);
			deathScreen = false;
		}
		health = 100;
		dead = false;
		if (flagCatured != "") {
			GameObject.Find (flagCatured).GetComponent<flag> ().dropFlag ();
			flagCatured = "";
		}

		if (team == 1) {
			transform.position = GameObject.Find ("spawn1").transform.position;
		}
		if (team == 2) {
			transform.position = GameObject.Find ("spawn2").transform.position;
		}
	}

	public void startGame () {
		if (isServer)
			game.CmdAnnounceStart ();
		gameIsRunning = true;
		GetComponent<FirstPersonController> ().m_MouseLook.lockCursor = true;
		StartCoroutine (resetPosition (false));
		GameObject.Find ("lobby").SetActive (false);
		GameObject.Find ("networker").GetComponent<NetworkManagerHUD> ().showGUI = false;
	}

	bool whoPickedTeams () {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("player");
		string playersWhoPickedTeam = "";
		foreach (var player in players) {
			if (player.GetComponent<playerStats> ().team == 0)
				playersWhoPickedTeam += player.GetComponent<playerStats> ().playerName + ", ";
		}
		if (playersWhoPickedTeam != "") {
			GUI.Box (new Rect (Screen.width / 2 - 125, 10, 250, 25), playersWhoPickedTeam + "did not pick team.");
			return false;
		} else {
			return true;
		}
	}


	public void giveMoney (int amount) {
		money += amount;

	}


	[Command]
	public void CmdStartGame () {
		RpcStartGame ();
	}

	[ClientRpc]
	public void RpcStartGame () {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("player");
		foreach (var player in players) { // finding own player...
			if (player.GetComponent<hit> ().isActiveAndEnabled) {
				player.GetComponent<playerStats> ().startGame ();
				break;
			}
		}
	}


	[Command]
	public void CmdUpdateTeam (NetworkInstanceId netId, int team) {
		//print ("Server: Player " + netId + " would like to be on team: " + team);
		RpcUpdateTeam (netId, team);
	}

	[ClientRpc]
	public void RpcUpdateTeam (NetworkInstanceId netId, int team) {
		//print ("Client: Player " + netId + " would like to be on team: " + team);
		GameObject player = GameObject.Find (gameController.PLAYER + netId);
		playerStats stats = player.GetComponent<playerStats> ();
		stats.team = team;
		GetComponent<MeshRenderer> ().material = (team == 1) ? team1Color : team2Color;
	}
		

}
