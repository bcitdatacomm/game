using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: IMPLEMENT Weapon drop as delete.  Do a check for pick up.

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

    void Start()
    {
        Debug.Log("Player start");
        this.Health = 100;
        this.Armor = 0;
        this.FiredShots = new Stack<Bullet>();
        this.TrackedShots = new Dictionary<int, Bullet>();
        net = Vector3.zero;


        sound = GetComponent<AudioSource>();
        sound.Play();

        // TEST CODE
        // GameObject Pistol = Instantiate(Resources.Load("Pistol", typeof(GameObject))) as GameObject;
        // currentGun = Pistol.GetComponent("Gun") as Gun;

        GameObject inventGameObj = GameObject.Find("Inventory"); // Inventory game object
        inventory = inventGameObj.transform.GetComponent<Inventory>();

        // Init spell list
        spells = new Spell[3];

        for (int i = 0; i < 3; i++)
        {
            // spells[i] = this.transform.GetChild(IDX_PREFAB_PLYR + 1 + i).GetComponent<Spell>();
            //            spells[i] = inventGameObj.transform.GetChild(1 + i).GetComponent<Spell>();
            spells[i] = inventGameObj.transform.GetChild(1 + i).GetComponent<Spell>();
            spells[i].enabled = false; // initially disable spell use
        }
        // Init spell list

        lastPickUp = DateTime.Now;
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
        anim.SetBool("Moving", false);
        move();
        turn();
        ManualReload();
        SwitchSpell();
        DebugLogger(); // Testing purposes.
    }

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

    void ManualReload()
    {
        // TODO: Minor issue, reloads multiple times, perhaps multiple ticks?
        if (Input.GetKey("r"))
        {
            Debug.Log("Reloading!");
            GameObject GunRef = GameObject.FindGameObjectWithTag("currentWeapon");
            Gun reloadGun = GunRef.GetComponentInChildren<Gun>();

            if (reloadGun != null)
            {
                reloadGun.Reload();
                Debug.Log(GunRef.name);
                sound.PlayOneShot(reload);
            }

        }
    }

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

    void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown("e") && ((int)(DateTime.Now - lastPickUp).TotalMilliseconds > 10))
        {
            lastPickUp = DateTime.Now;

            Debug.Log("E pressed");
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

    public byte[] getInventory()
    {
        byte[] checkInventory = new byte[5] { inventory.getWeapon(), inventory.getSpell(1), inventory.getSpell(2), inventory.getSpell(3), inventory.getCurrentSpell() };
        return checkInventory;
    }
}
