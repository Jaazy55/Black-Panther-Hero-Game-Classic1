using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEnable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.gameObject.SetActive (true);
		Invoke ("TextFalse", 3f);
	}
	void TextFalse()
	{
		this.gameObject.SetActive (false);

	}
	 


}
