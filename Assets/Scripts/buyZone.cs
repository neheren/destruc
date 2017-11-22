using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class buyZone : MonoBehaviour {

	public GameObject gui;
	public bool buyMenuOpen;
	GameObject player;
	public GUIStyle buttonStyle;
	playerStats stats;
	float timeToShoot;
	public int hotdogPrice = 100;
	public int sentryPrice = 1000;

	struct item {
		public int price;
		public GameObject gun;
		public int amount;
		public int ammoPrice;

		public item (int _price, GameObject _gun, int _amount, int _ammoPrice) {
			price = _price;
			gun = _gun;
			amount = _amount;
			ammoPrice = _ammoPrice;
		}

	}

	List<item> items;

	void Start () {
		buyMenuOpen = false;
		items = new List<item> ();
	}

	void Update () {
		if (buyMenuOpen)
			timeToShoot += Time.deltaTime;
		if ((Input.GetKeyDown ("e") || Input.GetKeyDown (KeyCode.Escape)) && mayShoot (0.3f))
			closeBuyMenu ();
	}

	public bool mayShoot (float reloadTime) {
		if (timeToShoot > reloadTime) {
			timeToShoot = 0;
			return true;
		} else {
			return false;
		}
	}


	public void openBuyMenu (GameObject _player) {
		player = _player;
		stats = player.GetComponent<playerStats> ();
		player.GetComponent<FirstPersonController> ().m_MouseLook.SetCursorLock (false);
		player.GetComponent<FirstPersonController> ().m_MouseLook.XSensitivity = 0;
		player.GetComponent<FirstPersonController> ().m_MouseLook.YSensitivity = 0;
		buyMenuOpen = true;
		_player.GetComponent<hit> ().disableMouse1 = true;

		int gunAmount = player.transform.GetChild (1).transform.childCount;
		items = new List<item> ();
		for (int i = 0; i < gunAmount; i++) {
			GameObject item = player.transform.GetChild (1).transform.GetChild (i).gameObject;
			GameObject gun = item;
			print (gun.name);
			int price = item.GetComponent<itemInfo> ().price;
			int ammoPrice = item.GetComponent<itemInfo> ().ammoPrice;
			items.Add (new item (price, gun, 1, ammoPrice));
		}
		gui.SetActive (true);
	}

	public void closeBuyMenu () {
		player.GetComponent<FirstPersonController> ().m_MouseLook.SetCursorLock (true);
		player.GetComponent<FirstPersonController> ().m_MouseLook.XSensitivity = 1;
		player.GetComponent<FirstPersonController> ().m_MouseLook.YSensitivity = 1;
		buyMenuOpen = false;
		player.GetComponent<hit> ().disableMouse1 = false;
		gui.SetActive (false);
	}

	public void buyItem (string item) {
		int playerMoney = stats.money;
		item requesteditem = getItemFromName (item);
		print ("player buying " + item);
		if (requesteditem.price < playerMoney) {
			player.GetComponent<playerStats> ().money -= requesteditem.price;
			player.GetComponent<hit> ().guns.Add (requesteditem.gun);
		}
	}

	public void buyAmmo (GameObject item) {
		int playerMoney = stats.money;
		item requesteditem = getItemFromName (item.name);
		print ("player buying ammo: " + item.name);
		if (requesteditem.ammoPrice < playerMoney) {
			player.GetComponent<playerStats> ().money -= requesteditem.ammoPrice;
			itemInfo itemI = item.GetComponent<itemInfo> ();
			itemI.shots += itemI.magSize;
		}
	}

	item getItemFromName (string _item) {
		foreach (var buyItem in items) {
			if (buyItem.gun.name == _item) {
				return buyItem;
			}
		}
		return new item (0, null, 0, 0);
	}

	void OnGUI () {
		if (buyMenuOpen) {
			buttonStyle.normal.textColor = Color.black;

			if (GUI.Button (new Rect (500, 100, 200, 30), "Hotdog ", buttonStyle)) {
				if (stats.health > 70 && stats.health < 90 && stats.money > hotdogPrice) {
					stats.health += 10;
					stats.money -= hotdogPrice;
				} else {
					print ("cant buy hotdog");
				}
			}

			if (GUI.Button (new Rect (500, 140, 200, 30), "Sentry ", buttonStyle)) {
				if (stats.money > sentryPrice) {
					player.GetComponent<hit> ().sentryAmount++;
					stats.money -= sentryPrice;
					
				} else {
					print ("cant buy sentry");
				}
			}

			GUI.Box (new Rect (100, 60, 200, 30), "Money: " + stats.money, buttonStyle);
			int playerMoney = player.GetComponent<playerStats> ().money;
			for (int i = 0; i < items.Count; i++) {
				buttonStyle.normal.textColor = Color.black;
				if (!player.GetComponent<hit> ().guns.Contains (items [i].gun) && items [i].price < playerMoney) {
					if (GUI.Button (new Rect (100, 40 * i + 100, 200, 30), items [i].gun.name, buttonStyle)) {
						buyItem (items [i].gun.name);
					}
				} else {
					buttonStyle.normal.textColor = new Color (0.2f, 0.2f, 0.2f);
					if (GUI.Button (new Rect (100, 40 * i + 100, 200, 30), items [i].gun.name + " - " + items [i].gun.GetComponent<itemInfo> ().shots.ToString () + " shots", buttonStyle)) {
					}
				}
			}

			for (int i = 0; i < items.Count; i++) {
				buttonStyle.normal.textColor = Color.black;
				if (GUI.Button (new Rect (320, 40 * i + 100, 100, 30), "Ammo " + items [i].ammoPrice + " $", buttonStyle)) {
					buyAmmo (items [i].gun);
				}
			}
		}
	}
}
