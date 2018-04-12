using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	PlayerCamera.cs
--
--	PROGRAM:		Player
--
--	FUNCTIONS:
--	         			void Start()
--                  			void Update()
--
--	DATE:			Mar 15, 2018
--
--	REVISIONS:
--
--	DESIGNERS:		John Tee
--
--	PROGRAMMER:	John Tee
--
--	NOTES:
--     Camera that follows the player: player is always in the center
--     of the screen for the user.
---------------------------------------------------------------------------------------*/

public class PlayerCamera : MonoBehaviour
{
    //Public variable to store a reference to the player game object

    public GameObject Player;

    //Private variable to store the offset distance between the player and camera
    private Vector3 offset;

    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		Start()
       --
       -- DATE: 			Mar 15, 2018
       --
       -- REVISIONS:
       --
       -- DESIGNER: 		John Tee
       --
       -- PROGRAMMER: 	John Tee
       --
       -- INTERFACE: 		Start()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Initializes the camera to stay at the same distance from the player.
       -------------------------------------------------------------------------------------------------*/

    void Start()
    {
        offset = transform.position - Player.transform.position;
    }

    /*-------------------------------------------------------------------------------------------------
     -- FUNCTION: 		Update()
     --
     -- DATE: 			Mar 15, 2018
     --
     -- REVISIONS:
     --
     -- DESIGNER: 		John Tee
     --
     -- PROGRAMMER: 	John Tee
     --
     -- INTERFACE: 		Update()
     --
     -- RETURNS: 		void
     --
     -- NOTES:
     -- Updates the camera position to keep up with player coordinates.
     -------------------------------------------------------------------------------------------------*/

    void Update()
    {
        transform.position = Player.transform.position + offset;
    }
}
