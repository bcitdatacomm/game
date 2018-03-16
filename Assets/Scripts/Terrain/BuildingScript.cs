using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour {

    // Use this for initialization
    public Transform brick;
    public Material skyone;
    public Texture2D myGUITexture;

    void Start()
    {
        myGUITexture = (Texture2D)Resources.Load("Assets/Scenery/Rocks Pack/Enviro/Grounds/ground1.png");
        //RenderSettings.skybox = (Material)myGUITexture;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
