using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public string displayName;
    public Transform playerPos;
    public float x_coordinate;
    public float y_coordinate;
    public int health = 100;
    public float moveSpeed = 5.0f;
    //public Weapon[] equippedWeapon; // need to have a Weapon class first (parent of gun and spell)
    public Player lastHitBy; // to know who killed you

    Rigidbody playerRigidbody;
    Vector3 movement;

    // Use this for initialization
    void Start () {
        x_coordinate = playerPos.transform.position.x;
        y_coordinate = playerPos.transform.position.y;
        playerRigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        x_coordinate = playerPos.transform.position.x;
        y_coordinate = playerPos.transform.position.y;
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Store the input axes.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // deal with playerMovement, not implemented currently
        //Move(h, v);
    }

    /* probably need a PlayerMovement class which deal with player movement
    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);
        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * moveSpeed * Time.deltaTime;
        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + movement);
    }*/
}
