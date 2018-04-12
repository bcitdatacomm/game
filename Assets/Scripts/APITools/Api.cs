using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighnoonTools;

public class Api : MonoBehaviour {

	public HighnoonManager API { get; set; }

	// Use this for initialization
	void Start () {
		API = new HighnoonManager("http://159.65.109.194");
		DontDestroyOnLoad(this.gameObject);
	}
}
