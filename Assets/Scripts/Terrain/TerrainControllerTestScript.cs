using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InitGuns;

// Assign this to an empty game object in order to test the Terrain Controller
public class TerrainControllerTestScript : MonoBehaviour
{

    // Use this for initialization of the Terrain Object, and for testing function calls
    void Start()
    {
        InitRandomGuns ic = new InitRandomGuns(30);

        TerrainController tc = new TerrainController();
        tc.LoadGuns(ic.compressedpcktarray);
        tc.GenerateEncoding();
        tc.Instantiate();
    }

    // Unused
    void Update()
    {

    }
}
