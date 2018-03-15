﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Health;

    public int Armor;

    public float MovementSpeed;

    public Spell[] Spells;

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Animator anim;                      // Reference to the animator component.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    float camRayLength = 100f;          // The length of the ray from the camera into the scene.

    void Start()
    {
        Debug.Log("Player start");
        this.Health = 100;
        this.Armor = 0;
        this.Spells = new Spell[3];
    }

    void Awake()
    {
        // Create a layer mask for the floor layer.
        floorMask = LayerMask.GetMask("Floor");

        // Set up references.
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
		move();
        turn();
    }

    void move()
    {
		if (Input.GetKey("w"))
		{
			this.transform.position = this.transform.position + new Vector3 (0, 0, .1f);
		}
		if (Input.GetKey("s"))
		{
			this.transform.position = this.transform.position + new Vector3 (0, 0, -.1f);
		}
		if (Input.GetKey("a"))
		{
			this.transform.position = this.transform.position + new Vector3 (-.1f, 0, 0);
		}
		if (Input.GetKey("d"))
		{
			this.transform.position = this.transform.position + new Vector3 (.1f, 0, 0);
		}
    }

    void turn()
    {
		Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane plane=new Plane(Vector3.up, Vector3.zero);
		float distance;
		if(plane.Raycast(ray, out distance))
		{
			Vector3 target=ray.GetPoint(distance);
			Vector3 direction=target-transform.position;
			float rotation=Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg;
			transform.rotation=Quaternion.Euler(0, rotation, 0);
		}
    }
}