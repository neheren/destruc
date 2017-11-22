using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour
{

	Animation animations;

	// Use this for initialization
	void Start ()
	{
		animations = GetComponent<Animation> ();
		animations.Play ("gun");
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void animateGunUp ()
	{
		try {
			animations.Stop ("gun");
			animations.Play ("bringGunUp");
			animations.PlayQueued ("gun");
		} catch {
			
		}

	}

	public void shoot ()
	{
		if (!animations.IsPlaying ("bringGunUp")) {
			if (animations.IsPlaying ("shot2"))
				animations.Stop ("shot2");
			animations.Stop ("gun");
			animations.Play ("shot2");
			animations.PlayQueued ("gun");
		}

	}
}
