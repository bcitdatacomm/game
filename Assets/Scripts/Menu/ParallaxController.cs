using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour {

	Camera mainCamera;
	public Transform target;

	Vector3 mousePos;
	Vector3 posByOrigin;

	public int xBound;
	public int yBound;

	public int innerBound;

	Vector3 cameraPos;

	public int shift;

	// Use this for initialization
	void Start () 
	{
		mainCamera = Camera.main;

		mousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () 
	{
		mousePos = Input.mousePosition;	
		posByOrigin.x = mousePos.x - (Screen.width/2);
		posByOrigin.y = mousePos.y - (Screen.height/2);

		if (mousePos.x >= 0 && mousePos.y >= 0) 
		{
			if (posByOrigin.x > innerBound && cameraPos.x < xBound) 
			{
				transform.Translate (shift,0,0);
				cameraPos.x += shift;
			}
			else if (posByOrigin.x < -innerBound && cameraPos.x > -xBound) 
			{
				transform.Translate (-shift,0,0);
				cameraPos.x -= shift;
			}
			else if (posByOrigin.y > innerBound && cameraPos.y < yBound) 
			{
				transform.Translate (0,shift,0);
				cameraPos.y += shift;
			}
			else if (posByOrigin.y < -innerBound && cameraPos.y > -yBound) 
			{
				transform.Translate (0,-shift,0);
				cameraPos.y -= shift;
			}
		}



		/*
		if (Input.GetMouseButtonDown (0)) 
		{
			Debug.Log ("Left mouse button pressed.");

			Debug.Log (mousePos.x);
			Debug.Log (mousePos.y);

			Debug.Log (posByOrigin.x);
			Debug.Log (posByOrigin.y);
		}
		*/


		transform.LookAt (target);
	}
}
