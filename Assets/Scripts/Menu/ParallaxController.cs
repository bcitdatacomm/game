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
	int actualShift;

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
				actualShift = (int) posByOrigin.x / shift;

				transform.Translate (actualShift,0,0);
				cameraPos.x += actualShift;
			}
			else if (posByOrigin.x < -innerBound && cameraPos.x > -xBound) 
			{
				actualShift = (int) posByOrigin.x / shift;

				transform.Translate (actualShift,0,0);
				cameraPos.x += actualShift;
			}

			if (posByOrigin.y > innerBound && cameraPos.y < yBound) 
			{
				actualShift = (int) posByOrigin.y / shift;

				transform.Translate (0,actualShift,0);
				cameraPos.y += actualShift;
			}
			else if (posByOrigin.y < -innerBound && cameraPos.y > -yBound) 
			{
				actualShift = (int) posByOrigin.y / shift;

				transform.Translate (0,actualShift,0);
				cameraPos.y += actualShift;
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
