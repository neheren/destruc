using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelLoader : MonoBehaviour {

	public string level;

	void Start () {
		StartCoroutine (loadAsync (level));
	}

	IEnumerator loadAsync (string level) {
		AsyncOperation operation = SceneManager.LoadSceneAsync (level); 
		while (!operation.isDone) {
			print (Mathf.Clamp01 (operation.progress / .9f));
			yield return new WaitForSeconds (0.1f);
		}
	}
}
