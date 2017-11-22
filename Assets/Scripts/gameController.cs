using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class gameController : NetworkBehaviour {
	public const string CUBE = "cube ";
	public const string CAR = "car ";
	public const string PLAYER = "player ";
	public const string SENTRY = "sentry ";
	public const string FLAG = "flag ";
	public static string playerName = "";
	public int killAward = 200;
	GameObject localPlayer;
	public List <string> killFeed;
	[SyncVar]
	public string killFeedString;
	[SyncVar]
	public string anouncement;
	[SyncVar]
	public int round = 1;



	void Start () {
	}

	void findLocalPlayer () {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("player");
		foreach (var player in players) {
			if (player.GetComponent<playerStats> ().isLocalPlayer) {
				localPlayer = player;
				break;
			}
		}	
	}

	public static bool isDead (int playerHealth, int damage) {
		if (playerHealth - damage <= 0 && playerHealth - damage > -damage)
			return true;
		else
			return false;
	}

	[Command]
	public void CmdAnounceDeath (string killedPlayerId, string killedById, string with) {
		playerStats killedPlayer = GameObject.Find (killedPlayerId).GetComponent<playerStats> ();
		playerStats killedBy = GameObject.Find (killedById).GetComponent<playerStats> ();

		RpcGiveMoneyToTeam (killAward, killedBy.team);

		if (killedBy == killedPlayer) {
			string msg = killedPlayer.playerName + " commited suicide with a " + with;
			CmdAddMessageToKillFeed (msg);
			CmdAnnouncement (msg);
			killedPlayer.deaths++;
			return;
		}
		
		if (killedBy.team == killedPlayer.team) {
			string msg = killedPlayer.playerName + " killed his team mate " + killedBy.playerName + " with a " + with;
			CmdAddMessageToKillFeed (msg);
			CmdAnnouncement (msg);
			killedPlayer.deaths++;
			killedBy.kills--;
			return;
		}

		killedBy.kills++;
		killedPlayer.deaths++;


		CmdAddMessageToKillFeed (killedPlayer.playerName + " was killed by " + killedBy.playerName + " with a " + with);
	}

	[ClientRpc]
	void RpcPrint (string msg) {
		print (msg);
	}


	[Command]
	public void CmdAnnounceStart () {
		RpcFreezePlayer (false);
		anouncement = "Round " + round + " has started!";
		StartCoroutine (removeAnouncement (false));
	}


	[Command]
	public void CmdAnnouncement (string announce) {
		anouncement = announce;
		StartCoroutine (removeAnouncement (false));
	}

	[Command]
	public void CmdAnnounceFlag (string playerWhoCapped, int team) {
		round++;
		anouncement = playerWhoCapped + " from team " + team + " captured the flag!";
		StartCoroutine (removeAnouncement (true));
		RpcFreezePlayer (true);

	}

	public static gameController getController () {
		return GameObject.FindWithTag ("controller").GetComponent<gameController> ();
	}


	[Command]
	void CmdAddMessageToKillFeed (string msg) {
		killFeed.Add (msg);	
		getController ().updateKillFeed ();
		getController ().removeMessageFromKillFeedInstance ();
	}

	public void removeMessageFromKillFeedInstance () {
		StartCoroutine (removeMessageFromKillFeed ());

	}

	IEnumerator removeMessageFromKillFeed () {
		yield return new WaitForSeconds (5);
		killFeed.RemoveAt (0);
		updateKillFeed ();
	}

	IEnumerator removeAnouncement (bool resetGame) {
		if (resetGame) {
			string originalAnounce = anouncement;
			for (int i = 0; i < 5; i++) {
				anouncement = originalAnounce + "\nRestarting in " + (5 - i);
				yield return new WaitForSeconds (1);
			}
			CmdresetGame ();
		} else {
			yield return new WaitForSeconds (5);
		}
		anouncement = "";
	}


	[ClientRpc]
	void RpcFreezePlayer (bool freeze) {
		findLocalPlayer ();
		localPlayer.GetComponent<hit> ().disableMouse1 = freeze;
		localPlayer.GetComponent<FirstPersonController> ().m_MouseLook.smooth = freeze;
	}

	[Command]
	void CmdresetGame () {
		RpcFreezePlayer (false);
		localPlayer.GetComponent<playerStats> ().health = 100;
		StartCoroutine (localPlayer.GetComponent<playerStats> ().resetPosition (false));
	}


	void updateKillFeed () {
		killFeedString = "";
		foreach (var kill in killFeed) {
			killFeedString += kill + "\n";
		}
	}

	[Command]
	void CmdAddKillToPlayer (string playerIdName, bool kill) {
		RpcAddKillToPlayer (playerIdName, kill);
	}

	[ClientRpc]
	void RpcAddKillToPlayer (string playerIdName, bool kill) {
		if (kill)
			GameObject.Find (playerIdName).GetComponent<playerStats> ().kills++;
		else
			GameObject.Find (playerIdName).GetComponent<playerStats> ().deaths++;
	}

	[ClientRpc]
	void RpcGiveMoneyToTeam (int amount, int team) {
		if (localPlayer.GetComponent<playerStats> ().team == team) {
			localPlayer.GetComponent<playerStats> ().giveMoney (amount);
		}
	}

}
