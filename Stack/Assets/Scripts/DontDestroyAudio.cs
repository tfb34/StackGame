using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyAudio : MonoBehaviour {
	public static DontDestroyAudio instance = null;
	// Use this for initialization
	void Awake(){
		//there's no audio
		if (instance == null)
			instance = this;
		//audio already exists and this instance is not the original
		else if (instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this.gameObject);
	}
}
