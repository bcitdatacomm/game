<<<<<<< 131a107a9179aa6c90348664d1212631c1cee1df
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assign this to an empty game object in order to test the Terrain Controller
public class TerrainControllerTestScript : MonoBehaviour
{

    // Use this for initialization of the Terrain Object, and for testing function calls
    void Start()
    {
        new TerrainController().Instantiate();
    }

    // Unused
    void Update()
    {

    }
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assign this to an empty game object in order to test the Terrain Controller
public class TerrainControllerTestScript : MonoBehaviour {

	// Use this for initialization of the Terrain Object, and for testing function calls
	void Start () {
        new TerrainController().Instantiate();
	}
	
	// Unused
	void Update () {
		
	}
}
>>>>>>> Created default constants, basic Instantiate(), default constructor, and test script for testing the controller
