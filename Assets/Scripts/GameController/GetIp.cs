using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	GetIp.cs
--
--	PROGRAM:		GameController
--
--	FUNCTIONS:
--				void Start()
--                     			void Update()
--
--	DATE:			Mar 28, 2018
--
--	REVISIONS:
--
--	DESIGNERS:		Benny Wang
--
--	PROGRAMMER:	Benny Wang
--
--	NOTES:
--   Allows getting and setting of the IP address of the client.
---------------------------------------------------------------------------------------*/

public class GetIp : MonoBehaviour
{

	public InputField MainInputField;
	public string Ip { get; set; }

	/*-------------------------------------------------------------------------------------------------
     -- FUNCTION: 		Start()
     --
     -- DATE: 			Mar 28, 2018
     --
     -- REVISIONS:
     --
     -- DESIGNER: 		Benny Wang
     --
     -- PROGRAMMER: 	Benny Wang
     --
     -- INTERFACE: 		Start()
     --
     -- RETURNS: 		void
     --
     -- NOTES:
     -- Empty initializer.
 -------------------------------------------------------------------------------------------------*/

	void Start ()
	{
	}

	/*-------------------------------------------------------------------------------------------------
	    -- FUNCTION: 		Update()
	    --
	    -- DATE: 			Mar 28, 2018
	    --
	    -- REVISIONS:
	    --
	    -- DESIGNER: 		Benny Wang
	    --
	    -- PROGRAMMER: 	Benny Wang
	    --
	    -- INTERFACE: 		Update()
	    --
	    -- RETURNS: 		void
	    --
	    -- NOTES:
	-- On every tick, grabs the IP address from the input field.
	-------------------------------------------------------------------------------------------------*/

	void Update ()
	{
		Ip = MainInputField.text;
		Debug.Log (Ip);
	}
}
