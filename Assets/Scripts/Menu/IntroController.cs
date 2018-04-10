/*------------------------------------------------------------------------------
--	Simple code used to run gun sounds when the mouse is clicked.
------------------------------------------------------------------------------*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour {

	public AudioSource gun1;
	public AudioSource gun2;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0))
		{
			gun1.Play ();
		}

	}
}
