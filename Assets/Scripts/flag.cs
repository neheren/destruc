using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flag : MonoBehaviour {
	public Vector3 originalPosition;
	public int team;

	public Transform moveTransform;
	public int captureProgress = 0;
	public int captureThreshold;
	public Vector3 flagOffset;
	bool flagDroppable = false;
	public float timeSinceCaptured = 0;
	public Material team1;
	public Material team2;
	public GameObject flagObject;

	void Start () {
		Material chosenColor = (team == 1) ? team1 : team2;
		flagObject.GetComponent<MeshRenderer> ().material = chosenColor;
		originalPosition = transform.position;
	}

	void Update () {
		if (moveTransform != null) {
			flagDroppable = true;
			transform.position = moveTransform.position + flagOffset;
			transform.rotation = moveTransform.rotation;
			timeSinceCaptured += Time.deltaTime;

		} else if (flagDroppable) {
			transform.position -= flagOffset + new Vector3 (0, 2, 0);
			flagDroppable = false;
			transform.rotation = Quaternion.Euler (0, 0, 0);

		}
	}

	void OnTriggerStay (Collider col) {
		if (moveTransform == null && col.transform.tag == "player") {
			captureProgress++;
			if (captureProgress > captureThreshold) {
				if (team != col.gameObject.GetComponent<playerStats> ().team) {
					captureFlag (col);
				} else {
					transform.position = originalPosition;
				}
			}
		}
	}

	public void captureFlag (Collider col) {
		col.gameObject.GetComponent<playerStats> ().flagCatured = this.name;
		timeSinceCaptured = 0;
		col.gameObject.GetComponent<hit> ().CmdAssignAuthority (col.name, this.name);
		moveTransform = col.transform.GetChild (1).transform;
	}

	public void dropFlag () {
		captureProgress = 0;
		moveTransform = null;
	}

	void OnTriggerExit (Collider col) {
		if (moveTransform == null && col.transform.tag == "player") {
			captureProgress = 0;
		}
	}
}
