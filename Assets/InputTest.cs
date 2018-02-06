using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}

	// Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Debug"))
        {
            Debug.Log("You found the debug button at time: " + Time.realtimeSinceStartup);
        }
        // In future should use this for space, inputString just returns a " " string 
        else if (Input.GetKeyDown("space"))
        {
            Debug.Log("Space key pressed");
        }
        else if (Input.GetKey(KeyCode.Return))
        {
            Debug.Log("Enter key down");
        }
        else if (Input.anyKeyDown)
        {
            // weird how its called input string
            // but its really a getchar member
            Debug.Log("A key was pressed:" + Input.inputString); 
        }
    }

    // FixedUpdate is called at irregular intervals every frame: a more irregular Update
    convention is to use only with a physics event
    void FixedUpdate()
    {
        //Debug.Log("FixedUpdate realTime: " + Time.realtimeSinceStartup);
    }
}
