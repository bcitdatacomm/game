/*------------------------------------------------------------------------------
--	APPLICATION:	c4981_Game.exe
--
--	CLASS:			ParallaxController
--
--	DATE:			March 13th, 2018
--
--	DESIGNER:		Morgan Ariss
--
--	PROGRAMMER:		Morgan Ariss
--
--	FUNCTIONS:		Start()
--					Update()
--
--	NOTES:
--		This class is used to controll the parallaxing of the main menu camera.
--		It uses UNITY functions to track the mouse and orient the camera based
--		on its position, while keeping it within set bounds to ensure that the
-		camera is not abel to see the side of the background.
--
------------------------------------------------------------------------------*/

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

	/*----------------------------------------------------------------------
	-- FUNCTION:	Start()
	--
	-- DATE:		March 15th, 2018
	--
	-- DESIGNER:	Morgan Ariss
	--
	-- PROGRAMMING:	Morgan Ariss
	--
	-- INTERFACE:	Start()
	--
	-- ARGUMENTS:
	--
	-- RETURNS:
	--
	-- NOTES:
	--	This function runs immediately on creation and simply sets the camera to
	--	be used, and the initial mouse postion.
	--
	----------------------------------------------------------------------*/
	void Start ()
	{
		mainCamera = Camera.main;

		mousePos = Input.mousePosition;
	}

	/*----------------------------------------------------------------------
	-- FUNCTION:	Start()
	--
	-- DATE:		March 15th, 2018
	--
	-- DESIGNER:	Morgan Ariss
	--
	-- PROGRAMMING:	Morgan Ariss
	--
	-- INTERFACE:	Start()
	--
	-- ARGUMENTS:
	--
	-- RETURNS:
	--
	-- NOTES:
	--	This function runs once every second in game. It tracks the mouses
	-- 	position and alters the orientation of the camera to give the effect of
	--	a parallaxing view. It ensures that the camera has not exceeded the
	-- 	boundaries set in the inspektor in UNITY. All of these values are editable
	--	from the editor.
	--
	----------------------------------------------------------------------*/
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
		transform.LookAt (target);
	}
}
