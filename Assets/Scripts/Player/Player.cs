using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	Player.cs
--
--	PROGRAM:		Player
--
--	FUNCTIONS:
--	         			public byte[] Weapon()
--	         			void Start()
--                  			void Awake()
--                  			void FixedUpdate()
--                  			void move()
--	         			void checkGun()
--                  			void turn()
--                  			void SwitchSpell()
--                  			void OnTriggerStay(Collider other)
--	        			byte[] getInventory()
--
--	DATE:		Mar 17, 2018
--
--	REVISIONS:	Mar 20, 2018 - added more fullsome
--			definitions of player behaviour.
--			Apr 10, 2018 - took out invinciblity
--
--	DESIGNERS:		Benny Wang, Li-Yan Tong, John Tee
--
--	PROGRAMMER:	Benny Wang, Li-Yan Tong, John Tee
--
--	NOTES:
--     This initializes a player with full health,  inventory for weapons
--     and spells, and the ability to move  around the game world.
---------------------------------------------------------------------------------------*/

public class Player : MonoBehaviour
{
    // Defined in Prefab
    public int Health;
    public int Armor;
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

    // For Item Logic
    private Inventory inventory;
    public Item currentItem;
    // Spell list
    private Spell[] spells;
    DateTime lastPickUp;
    public Gun currentGun;

    // Keep Track of Bullets
    public Stack<Bullet> FiredShots;
    public Dictionary<int, Bullet> TrackedShots;

    public bool triggerStay = false;
    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		Weapon()
        --
        -- DATE: 			Apr 2, 2018
        --
        -- REVISIONS:
        --
        -- DESIGNER: 		Benny Wang
        --
        -- PROGRAMMER: 	Benny Wang
        --
        -- INTERFACE: 		Weapon()
        --
        -- RETURNS: 		byte array for the player’s gun
        --
        -- NOTES:
        -- Returns a player’s weapon (if it exists or not).
        -------------------------------------------------------------------------------------------------*/

    public byte[] Weapon
    {
        get
        {
            Item gun = this.inventory.items[0];

            if (gun == null)
            {
                return new byte[5];
            }

            byte[] tmp = new byte[5];

            Array.Copy(BitConverter.GetBytes(gun.ID), 0, tmp, 0, 4);
            tmp[4] = gun.Type;

            return tmp;
        }
    }
    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		Start()
       --
       -- DATE: 			Mar 17, 2018
       --
       -- REVISIONS:		Mar 20, 2018 - changed default movement speed,
       -- 				added empty inventory, audio
       --
       -- DESIGNER: 		Benny Wang, Li-Yan Tong, John Tee, Anthony Vu
       --
       -- PROGRAMMER: 	Benny Wang, Li-Yan Tong, John Tee, Anthony Vu
       --
       -- INTERFACE: 		Start()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Initializes a player with full health and an empty inventory. Enables
       -- bullet and inventory pickup tracking.
       -------------------------------------------------------------------------------------------------*/

    void Start()
    {
        Debug.Log("Player start");
        this.Health = 100;
        this.Armor = 0;
        this.FiredShots = new Stack<Bullet>();
        net = Vector3.zero;
        net += this.transform.position;

        sound = GetComponent<AudioSource>();
        sound.Play();

        // TEST CODE
        // GameObject Pistol = Instantiate(Resources.Load("Pistol", typeof(GameObject))) as GameObject;
        // currentGun = Pistol.GetComponent("Gun") as Gun;

        GameObject inventGameObj = GameObject.Find("Inventory"); // Inventory game object
        inventory = inventGameObj.transform.GetComponent<Inventory>();

        // Init spell list
        // spells = new Spell[3];

        // for (int i = 0; i < 3; i++)
        // {
        //     // spells[i] = this.transform.GetChild(IDX_PREFAB_PLYR + 1 + i).GetComponent<Spell>();
        //     //            spells[i] = inventGameObj.transform.GetChild(1 + i).GetComponent<Spell>();
        //     spells[i] = inventGameObj.transform.GetChild(1 + i).GetComponent<Spell>();
        //     spells[i].enabled = false; // initially disable spell use
        // }
        // Init spell list

        lastPickUp = DateTime.Now;
    }
    /*-------------------------------------------------------------------------------------------------
      -- FUNCTION: 		Awake()
      --
      -- DATE: 			Mar 20, 2018
      --
      -- REVISIONS:
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
      -- Animates the player.
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
       -- DATE: 			Mar 17, 2018
       --
       -- REVISIONS:		Mar 20, 2018 - added reload, spell methods to
       --				include in tick update.
       --				Apr 1, 2018 - added animation checks
       --				Apr 10, 2018 - added death
       --
       -- DESIGNER: 		Benny Wang, Li-Yan Tong, Anthony Vu
       --
       -- PROGRAMMER: 	Li-Yan Tong, Anthony Vu
       --
       -- INTERFACE: 		FixedUpdate()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Main method which updates player behaviour each tick.
       -------------------------------------------------------------------------------------------------*/

    void FixedUpdate()
    {
        if (this.Health == 0)
        {
            Debug.Log("I Dead");
        }

        anim.SetBool("Moving", false);
        move();
        turn();
        checkGun();
        SwitchSpell();
        DebugLogger(); // Testing purposes.
    }
    /*-------------------------------------------------------------------------------------------------
      -- FUNCTION: 		move()
      --
      -- DATE: 			Mar 17, 2018
      --
      -- REVISIONS:		Mar 20, 2018 - changed method of grabbing
      --				movement directions, reloading from the user.
      --				Mar 30, 2018 - added y position update
      --
      -- DESIGNER: 		Benny Wang, John Tee
      --
      -- PROGRAMMER: 	Benny Wang, John Tee
      --
      -- INTERFACE: 		move()
      --
      -- RETURNS: 		void
      --
      -- NOTES:
      -- Moves the player in specific direction based on keyboard input.
      -------------------------------------------------------------------------------------------------*/

    void move()
    {
        if (Input.GetKey("w"))
        {
            this.transform.position = this.transform.position + new Vector3(0, 0, MovementSpeed);
            net = net + new Vector3(0, 0, MovementSpeed);
            anim.SetBool("Moving", true);
        }
        if (Input.GetKey("s"))
        {
            this.transform.position = this.transform.position + new Vector3(0, 0, -MovementSpeed);
            net = net + new Vector3(0, 0, -MovementSpeed);
            anim.SetBool("Moving", true);
        }
        if (Input.GetKey("a"))
        {
            this.transform.position = this.transform.position + new Vector3(-MovementSpeed, 0, 0);
            net = net + new Vector3(-MovementSpeed, 0, 0);
            anim.SetBool("Moving", true);
        }
        if (Input.GetKey("d"))
        {
            this.transform.position = this.transform.position + new Vector3(MovementSpeed, 0, 0);
            net = net + new Vector3(MovementSpeed, 0, 0);
            anim.SetBool("Moving", true);
        }
        Vector3 pos = this.transform.position;
        pos.y = 0;
        this.transform.position = pos;

    }

    void FootR()
    {

    }

    void FootL()
    {

    }
    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		CheckGun()
        --
        -- DATE: 			Apr 3, 2018
        --
        -- REVISIONS:
        --
        -- DESIGNER: 		Benny Wang, John Tee
        --
        -- PROGRAMMER: 	John Tee
        --
        -- INTERFACE: 		CheckGun()
        --
        -- RETURNS: 		void
        --
        -- NOTES:
        -- Checks if the player has a gun and allows player to shoot or reload.
        -------------------------------------------------------------------------------------------------*/

    void checkGun()
    {
        GameObject GunObject = GameObject.FindGameObjectWithTag("currentWeapon");
        Gun gun = GunObject.GetComponentInChildren<Gun>();

        if (gun != null)
        {
            // Manual Reload
            // TODO: Minor issue, reloads multiple times, perhaps multiple ticks?
            if (Input.GetKey("r"))
            {
                Debug.Log("Reloading!");

                gun.Reload();
                Debug.Log(gun.name);
                sound.PlayOneShot(reload);
            }

            gun.CheckShoot();
            gun.CheckReload();
        }
    }
    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		turn()
       --
       -- DATE: 			Mar 17, 2018 - created
       --
       -- REVISIONS:		Mar 20, 2018 - added logic to turn with mouse
       --
       -- DESIGNER: 		Jeffrey Chou, John Tee
       --
       -- PROGRAMMER: 	John Tee
       --
       -- INTERFACE: 		turn()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Turns the player in the direction of the mouse movement.
       -------------------------------------------------------------------------------------------------*/

    void turn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance) + net;
            Vector3 direction = target - transform.position;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }
    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		SwitchSpell()
       --
       -- DATE: 			Mar 20, 2018
       --
       -- REVISIONS:
       --
       -- DESIGNER: 		Jeremy Lee, Li-Yan Tong
       --
       -- PROGRAMMER: 	Jeremy Lee
       --
       -- INTERFACE: 		SwitchSpell()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Changes the current spell if they are in the player’s inventory, based
       -- on keyboard input.
       -------------------------------------------------------------------------------------------------*/

    void SwitchSpell()
    {
        // Spell switching
        int slotCurrentSpell = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1)) // Press Key "1"
        {
            slotCurrentSpell = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Press Key "2"
        {
            slotCurrentSpell = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // Press Key"3"
        {
            slotCurrentSpell = 2;
        }

        if (slotCurrentSpell >= 0)
        {
            for (int i = 0; i < 3; i++)
            {
                spells[i].enabled = (i == slotCurrentSpell) ? true : false;
            }
            Debug.Log("Switch to spell #" + (slotCurrentSpell + 1));
            this.inventory.CurrentSpell = slotCurrentSpell + 1; // value: 1 to 3
        }
        // Spell switching
    }

    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		OnTriggerStay()
       --
       -- DATE: 			Mar 21, 2018
       --
       -- REVISIONS:		Mar 21, 2018 - Get Event to work.
       --
       -- DESIGNER: 		Jeremy Lee, Matthew Shew
       --
       -- PROGRAMMER: 	Jeremy Lee
       --
       -- INTERFACE: 		OnTriggerStay(Collider other)
       --				other : the object the player collided with
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Adds a weapon to the player’s inventory; “picks up” the item for the
       -- player’s use based on keyboard input.
       -------------------------------------------------------------------------------------------------*/

    void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown("e")&& ((int)(DateTime.Now - lastPickUp).TotalMilliseconds > 10))
        {
            lastPickUp = DateTime.Now;

            Item item = other.GetComponent<Item>();

            GameObject itemObject = other.gameObject;

            GameObject WeaponSlot = GameObject.FindGameObjectWithTag("currentWeapon");

            // TODO: Create GameObjects in Player inventory with Tags for Spells
            //GameObject SpellSlot1 = GameObject.FindGameObjectWithTag("SpellSlot1");
            //GameObject SpellSlot2 = GameObject.FindGameObjectWithTag("SpellSlot2");
            //GameObject SpellSlot3 = GameObject.FindGameObjectWithTag("SpellSlot3");

            Debug.Log("Item collided: " + item.name);
            if (item != null && item.Category == 1) // Add a Weapon
            {
                Debug.Log("Weapon picked up: " + item.name);
                inventory.AddItem(item);
                string name = item.name;

                if (WeaponSlot.transform.childCount > 0) // Add Weapon GameObject to Player GameObject
                {
                    var temp = WeaponSlot.transform.GetChild(0);
                    temp.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
                    Collider collider = temp.transform.GetComponent<Collider>();
                    collider.enabled = true;
                    temp.transform.parent = null;
                }
                itemObject.transform.parent = WeaponSlot.transform;

                //item.isEquipped = true;
                item.transform.position = this.transform.position + new Vector3(0.2f, 1, 0);
                item.transform.rotation = this.transform.rotation;

                // Debug.Log(this.transform.name);
            }
            else if (item != null && item.Category == 2) // Add Spell
            {
                Debug.Log("picked up: " + item.name);
                inventory.AddItem(item);
                string name = item.name;

                // TODO: Logic to instantiate spell game object required.
            }
        }
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
    /*-------------------------------------------------------------------------------------------------
      -- FUNCTION: 		getInventory()
      --
      -- DATE: 			Mar 20, 2018
      --
      -- REVISIONS:		Mar 21, 2018 - add current to end of array
      --
      -- DESIGNER: 		Li-Yan Tong
      --
      -- PROGRAMMER: 	Li-Yan Tong
      --
      -- INTERFACE: 		getInventory()
      --
      -- RETURNS: 		a byte array of the player’s inventory
      --
      -- NOTES:
      -- Checks the inventory of the player, noting all weapon slots and the
      -- current “active” weapon.
      -------------------------------------------------------------------------------------------------*/

    public byte[] getInventory()
    {
        byte[] checkInventory = new byte[5] { inventory.getWeapon(), inventory.getSpell(1), inventory.getSpell(2), inventory.getSpell(3), inventory.getCurrentSpell() };
        return checkInventory;
    }
}
