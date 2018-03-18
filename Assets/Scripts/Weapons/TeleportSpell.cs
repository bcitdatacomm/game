using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpell : Spell {

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetButton("Fire2"))
        {

			float randomDistance = 25f;
			Vector3 teleportVector = new Vector3(Random.Range(-randomDistance, randomDistance),
				0, Random.Range(-randomDistance, randomDistance));
			transform.parent.position += teleportVector;
			transform.parent.GetComponent<Player>().net += teleportVector;
            Debug.Log("Spell used");


        }
	}
}
