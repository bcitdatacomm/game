using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public string DisplayName { get; set; }
    Transform playerPos { get; set; }
    public float xCoordinate { get; set; }
    public float yCoordinate { get; set; }
    public int Health = 100;
    public float MOVE_SPEED = 6.0f;

    // GameObject player;// never init or called this
    //GameObject otherPlayerGameObject { get; set; }
    string playerTag { get; set; }
    string pickupTag { get; set; }

    // not practical, must incre thru list for every new pickup
    GameObject[] pickUps { get; set; }
    bool playerInRange;
    float timer;
    public uint PlayerId { get; set; }

    //public Weapon[] equippedWeapon; // need to have a Weapon class first (parent of gun and spell)

    // to know who killed you
    public Player LastHitBy { get; set; }
    float TIME_BETWEEN_ATTACKS = 0.5f;
    Rigidbody playerRigidbody;
    Vector3 movement;

    private short GUN_BULLET_LIFETIME = 100;
    private int GUN_BULLET_SPEED = 6;

    private short MELEE_BULLET_LIFETIME = 75;
    private int MELEE_BULLET_SPEED = 3;

    private bool meleeMode;

    // Use this for initialization
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerPos = GetComponent<Transform>();
        //otherPlayer = GameObject.FindGameObjectWithTag("otherPlayer");
        playerTag = this.tag;
        pickupTag = "Item";
        // Random r = new Random();
        // playerId = r.Next(0, 32);
        PlayerId = 0;
    }

    void FixedUpdate()
    {
        Move();
        Turn();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (!meleeMode)
        {
            if (timer >= TIME_BETWEEN_ATTACKS && Input.GetButtonDown("Fire1"))
            {
                Attack();
            }
        }
        else
        {
            Debug.Log(playerInRange);
            if (timer >= TIME_BETWEEN_ATTACKS && Input.GetButtonDown("Fire1") && playerInRange)
            {
                Debug.Log("Attempting to hit");
                Attack();
            }
        }

    }

    void Move()
    {
        // Store the input axes.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(h, 0f, v);
        movement = movement.normalized * MOVE_SPEED * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
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


    void Attack()
    {
        short bulletLifeTime = GUN_BULLET_LIFETIME;
        int bulletSpeed = GUN_BULLET_SPEED;

        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        if (meleeMode)
        {
            bulletLifeTime = MELEE_BULLET_LIFETIME;
            bulletSpeed = MELEE_BULLET_SPEED;
            //bullet.GetComponent<MeshRenderer>().enabled = false; // (un)comment this line to display melee bullets
        }

        // Create the Bullet from the Bullet Prefab
        // Sets origin of bullets
        bullet.GetComponent<Bullet>().bulletLifeTime = bulletLifeTime;
        bullet.GetComponent<Bullet>().src = this.gameObject;
        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        // Spawn the bullet on the Clients
        //NetworkServer.Spawn(bullet);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(pickupTag))
        {
            meleeMode = !meleeMode;
            if (meleeMode)
                Debug.Log("picked up a melee weapon!");
            else
                Debug.Log("picked up a gun!");
        }
        //else if (other.gameObject == otherPlayer)
        //{
        //    Debug.Log("Entering");
        //    playerInRange = true;
        //    Debug.Log(playerInRange);
        //}
    }


    //void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject == otherPlayer)
    //    {
    //        Debug.Log("Leaving");
    //        playerInRange = false;
    //    }
    //}


}
