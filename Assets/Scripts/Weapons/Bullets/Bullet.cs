using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	Bullet.cs
--
--	PROGRAM:		Weapon
--
--	FUNCTIONS:
--				void Start()
--				void FixedUpdate()
--				public byte[] ToBytes()
--
--	DATE:			Mar 21, 2018
--
--	REVISIONS:		Mar 31, 2018 - added ToBytes function
--				Apr 10, 2018 - Bullet no longer destroyed
--
--	DESIGNERS:		Benny Wang, Li-Yan Tong
--
--	PROGRAMMER:	Li-Yan Tong, Benny Wang
--
--	NOTES:
--
---------------------------------------------------------------------------------------*/

public class Bullet : MonoBehaviour
{
    // Environment ID
    public int ID { get; set; }

    // Defined per Prefab
    // Packet Identification
    public byte Type;
    // Decrement Health
    public int Damage;
    // Projectile Speed
    public float Speed;
    // Seconds till Bullet Destroyed
    public float LifeTime;

    // Bullet Direction
    public Vector3 initPos;
    public Vector3 direction;
    /*-------------------------------------------------------------------------------------------------
      -- FUNCTION: 		Start()
      --
      -- DATE: 			Mar 21, 2018
      --
      -- REVISIONS:		Apr 10, 2018 - bullet no longer destroyed here
      --
      -- DESIGNER: 		Li-Yan Tong
      --
      -- PROGRAMMER: 	Li-Yan Tong, Benny Wang
      --
      -- INTERFACE: 		Start()
      --
      -- RETURNS: 		void
      --
      -- NOTES:
      -- Generates bullet ID, direction and a timer to destroy the bullet.
      -------------------------------------------------------------------------------------------------*/

    void Start()
    {
        // Generate Identification, Direction and Timer to destroy bullet object
        this.direction = this.direction.normalized;
        this.initPos = this.transform.position;

        // Server should do this for us
        // Destroy(this.gameObject, LifeTime);
        Debug.Log("Bullet spawned with id: " + this.ID);
    }
    /*-------------------------------------------------------------------------------------------------
      -- FUNCTION: 		FixedUpdate()
      --
      -- DATE: 			Mar 21, 2018
      --
      -- REVISIONS:		March 21, 2018
      --
      -- DESIGNER: 		Benny Wang, Li-Yan Tong
      --
      -- PROGRAMMER: 	Li-Yan Tong
      --
      -- INTERFACE: 		FixedUpdate()
      --
      -- RETURNS: 		void
      --
      -- NOTES:
      -- Moves the position of the bullet each tick update.
      -------------------------------------------------------------------------------------------------*/

    void FixedUpdate()
    {
        // Set Position of Bullet per Tick
        Vector3 tmp = direction;
        this.transform.position = this.transform.position + tmp * Speed;
        Vector3 p = this.transform.position;
        this.transform.position = new Vector3(p.x, 1, p.z);
    }
    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		ToBytes()
       --
       -- DATE: 			Mar 31, 2018
       --
       -- REVISIONS:		March 13, 2018
       --
       -- DESIGNER: 		Li-Yan Tong
       --
       -- PROGRAMMER: 	Li-Yan Tong
       --
       -- INTERFACE: 		ToBytes()
       --
       -- RETURNS: 		byte array of the bulletID
       --
       -- NOTES:
       -- Returns the bulletID byte array.
       -------------------------------------------------------------------------------------------------*/

    public byte[] ToBytes()
    {
        byte[] temp = new byte[5];
        Buffer.BlockCopy(BitConverter.GetBytes(this.ID), 0, temp, 0, 4);
        temp[4] = this.Type;
        return temp;
    }
}
