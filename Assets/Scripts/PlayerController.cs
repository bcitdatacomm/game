using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public string displayName;
    public Transform playerPos;
    public float x_coordinate;
    public float y_coordinate;
    public int health = 100;
    public float moveSpeed = 5.0f;
    //public Weapon[] equippedWeapon; // need to have a Weapon class first (parent of gun and spell)
    public GameObject lastHitBy; // to know which player killed you

    Rigidbody playerRigidbody;
    Vector3 movement;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    // Use this for initialization
    void Start () {
        playerPos = GetComponent<Transform>();
        //x_coordinate = playerPos.position.x;
        //y_coordinate = playerPos.position.y;
        playerRigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
    void Update()
    {
        // Store the input axes.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // deal with playerMovement
        Move(h, v);

        // turns towards the mouse
        Turn();

        x_coordinate = playerPos.position.x;
        y_coordinate = playerPos.position.y;
        float y_rot = playerPos.rotation.y;

        Debug.Log("x: " + x_coordinate + ", y: " + y_coordinate
            + ", y rot: " + y_rot);
        //send playerPos to server for x_coord, y_coord, and y rotation

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BulletAttack();
        }
    }

    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);
        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * moveSpeed * Time.deltaTime;
        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(playerPos.position + movement);
    }


    void Turn()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            // Set the player's rotation to this new rotation.
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    void BulletAttack()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        // Spawn the bullet on the Clients
        //NetworkServer.Spawn(bullet);

        // Destroy the bullet after 1 seconds; this acts as range for now
        Destroy(bullet, 1.0f);
    }
}
