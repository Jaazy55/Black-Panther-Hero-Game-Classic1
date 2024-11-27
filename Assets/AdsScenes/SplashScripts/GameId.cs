using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameId : MonoBehaviour {
	public string gameid= "";
	// Use this for initialization
	void Awake () {
		//PlayerPrefs.DeleteAll ();
		print(gameid);
		PlayerPrefs.SetString ("GameId",gameid);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
