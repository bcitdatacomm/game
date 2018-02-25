using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public string displayName;
    public Transform playerPos;
    public float x_coordinate;
    public float y_coordinate;
    public int health = 100;
    public float moveSpeed = 6.0f;
    // GameObject player;// never init or called this
    GameObject otherPlayerGameObject;
    string playerTag;
    string pickupTag;
    GameObject[] pickUps;// not practical, must incre thru list for every new pickup
    bool playerInRange;
    float timer;
    public uint playerId;
    public List<GameObject> otherPlayerList;
    
    //public Weapon[] equippedWeapon; // need to have a Weapon class first (parent of gun and spell)
    public Player lastHitBy; // to know who killed you
    float timeBetweenAttacks = 0.5f;
    Rigidbody playerRigidbody;
    Vector3 movement;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    private short gunBulletLifeTime = (short)100;
    private int gunBulletSpeed = 6;
    
    private short meleeBulletLifeTime = (short)75;
    private int meleeBulletSpeed = 3;

    private bool meleeMode = false;

    public Connection connection;

    // Use this for initialization
    void Start () {
        playerRigidbody = GetComponent<Rigidbody>();
        playerPos = GetComponent<Transform>();

        playerTag = this.tag;
        pickupTag = "Item";
        // Random r = new Random();
		// playerId = r.Next(0, 32);
		playerId = 0;

        connection = new Connection();
        char[] sdfa = { 'a', 's', 'd', 'f' };
        
        connection.ReadFromBuffer(sdfa);
    }
	
	void FixedUpdate()
    {
    	Move();
    	Turn();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= timeBetweenAttacks && Input.GetButtonDown("Fire1") )
        {
        	Attack();
        }
    }

    void Move()
    {
        // Store the input axes.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(h, 0f, v);
        movement = movement.normalized * moveSpeed * Time.deltaTime;    
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
    	short bulletLifeTime = gunBulletLifeTime;
    	int bulletSpeed = gunBulletSpeed;
        
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
    	if(meleeMode)
    	{
    		bulletLifeTime = meleeBulletLifeTime;
    		bulletSpeed = meleeBulletSpeed;
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
	// }
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
    }
    
}
