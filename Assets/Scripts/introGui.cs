using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class introGui : MonoBehaviour {

	// Use this for initialization
	public string nameText;
	public GUIStyle inputField;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI () {
		nameText = GUI.TextField (new Rect (10, 10, 200, 20), nameText, 20, inputField);
		gameController.playerName = nameText;
	}
}
