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
    GameObject player;
    GameObject otherPlayerObject;
    bool playerInRange;
    float timer;
    //public Weapon[] equippedWeapon; // need to have a Weapon class first (parent of gun and spell)
    public Player lastHitBy; // to know who killed you
    float timeBetweenAttacks = 0.5f;
    Rigidbody playerRigidbody;
    Vector3 movement;

    // Use this for initialization
    void Start () {

        playerRigidbody = GetComponent<Rigidbody>();
        x_coordinate = playerRigidbody.transform.position.x;
        y_coordinate = playerRigidbody.transform.position.y;

        //x_coordinate = playerPos.transform.position.x;
        //y_coordinate = playerPos.transform.position.y;
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
        x_coordinate = playerRigidbody.transform.position.x;
        y_coordinate = playerRigidbody.transform.position.y;
        //x_coordinate = playerPos.transform.position.x;
        //y_coordinate = playerPos.transform.position.y;
        timer += Time.deltaTime;
        if (timer >= timeBetweenAttacks && Input.GetButtonDown("Fire1") && playerInRange)
        {
            Attack();
        }

    }

    void Attack()
    {
        OtherPlayerHealth otherPlayerHealth = otherPlayerObject.GetComponent<OtherPlayerHealth>();
        //Player otherPlayer = otherPlayerObject.GetComponent<Player>();
        otherPlayerHealth.health -= 10;
        Debug.Log(otherPlayerHealth.health);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {

            playerInRange = true;
            otherPlayerObject = other.gameObject;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            playerInRange = false;
            otherPlayerObject = null;
        }
    }

    void FixedUpdate()
    {
        // Store the input axes.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h, 0f, v);
        movement = movement.normalized * 6 * Time.deltaTime;    
        playerRigidbody.MovePosition(transform.position + movement);
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
