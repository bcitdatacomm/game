using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	Enemy.cs
--
--	PROGRAM:		Player
--
--	FUNCTIONS:		void Start()
--				void Awake()
--				void FixedUpdate()
--
--
--	DATE:			Apr 2, 2018
--
--	REVISIONS:		Mar 21, 2018 refactored; consolidated spell functions
--
--	DESIGNERS:		Benny Wang & Anthony Vu
--
--	PROGRAMMER:	Benny Wang & Anthony Vu
--
--	NOTES:
--   	The Enemy class holds data required to move a “puppet” in the
--	game client representing other players.
---------------------------------------------------------------------------------------*/
public class Enemy : MonoBehaviour
{

    public float MovementSpeed;
    public AudioSource sound;
    public AudioClip reload;
    // A layer mask so that a ray can be cast just at gameobjects on the floor layer
    int floorMask;

    // The vector to store the direction of the player's movement.
    Vector3 movement;
    // Reference to the animator component.
    Animator anim;
    // Reference to the player's rigidbody.
    Rigidbody playerRigidbody;
    public Vector3 net;

    DateTime lastPickUp;
	Vector3 lastPos;
	Vector3 curPos;

  /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: 		Start()
    --
    -- DATE: 			Apr 2, 2018
    --
    -- REVISIONS:		Apr 2, 2018 - Initialize function
    --
    -- DESIGNER: 		Anthony Vu
    --
    -- PROGRAMMER: 	Anthony Vu
    --
    -- INTERFACE: 		Start()
    --
    -- RETURNS: 		void
    --
    -- NOTES:
    -- Initializes and updates the enemy players position and logs the timing
    -- of an enemy’s pickup.
    -------------------------------------------------------------------------------------------------*/

    void Start()
    {
        net = Vector3.zero;

        sound = GetComponent<AudioSource>();
        sound.Play();

        lastPickUp = DateTime.Now;
		curPos = transform.position;
		lastPos = curPos;
    }
    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		Awake()
        --
        -- DATE: 			Apr 2, 2018
        --
        -- REVISIONS:		Apr 2, 2018 - Initialize function
        --
        -- DESIGNER: 		Anthony Vu
        --
        -- PROGRAMMER: 	Anthony Vu
        --
        -- INTERFACE: 		Awake()
        --
        -- RETURNS: 		void
        --
        -- NOTES:
        -- Animates the enemy player.
        -------------------------------------------------------------------------------------------------*/

    void Awake()
    {
        // Create a layer mask for the floor layer.
        floorMask = LayerMask.GetMask("Floor");

        // Set up references.
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }
    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		FixedUpdate()
        --
        -- DATE: 			Apr 2, 2018
        --
        -- REVISIONS:		Apr 2, 2018 - Initialize function
        --
        -- DESIGNER: 		Anthony Vu
        --
        -- PROGRAMMER: 	Anthony Vu
        --
        -- INTERFACE: 		FixedUpdate()
        --
        -- RETURNS: 		void
        --
        -- NOTES:
        -- Moves the enemy if there is a difference between the last logged
        -- position and the current position.
        -------------------------------------------------------------------------------------------------*/

    void FixedUpdate()
    {
        anim.SetBool("Moving", false);
		float distance = Vector3.Distance(lastPos, curPos);
		if(distance != 0) {
			anim.SetBool("Moving", true);
		}
		lastPos = curPos;
		curPos = this.transform.position;

        DebugLogger(); // Testing purposes.
    }

    void FootR()
    {

    }

    void FootL()
    {

    }

    void DebugLogger()
    {
        if (Input.GetKey("t")) // Log Print tester
        {
            // For Testing Inventory byte array.
            // byte[] invent;
            // invent = getInventory();

            // Debug.Log(invent[0]);
            // Debug.Log(invent[1]);
            // Debug.Log(invent[2]);
            // Debug.Log(invent[3]);

            // For Testing Bullets
            //Bullet stackBullet = this.FiredShots.Peek();
            //Debug.Log("Player: Bullet Stack Stored: " + stackBullet.ID);
            //Debug.Log("Player: BULLET DICTIONARY: " + this.TrackedShots.ContainsKey(stackBullet.ID));
        }
    }
}
