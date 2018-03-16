using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Test_Child : Spell_Test {
    public int spellId;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire2"))
        {
            Debug.Log("Spell fired:" + spellId);

        }
	}
}
