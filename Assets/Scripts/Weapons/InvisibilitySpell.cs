using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilitySpell : Spell {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(Input.GetButton("Fire2"))
        {
			this.transform.parent.Find("char02").GetComponent<Renderer>().enabled = false;
        }
		if(Input.GetButton("Fire1"))
        {
			this.transform.parent.Find("char02").GetComponent<Renderer>().enabled = true;
        }
	}
}
