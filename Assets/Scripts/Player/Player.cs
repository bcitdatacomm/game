using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: IMPLEMENT Weapon drop as delete.  Do a check for pick up.

public class Player : MonoBehaviour
{
    public int Health;
    public int Armor;
    public float MovementSpeed;
    public Item currentItem;
    byte[] checkInventory;
    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Animator anim;                      // Reference to the animator component.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    AudioSource sound;
    public AudioClip reload;
    int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer
    float camRayLength = 100f;          // The length of the ray from the camera into the scene.
    public Vector3 net;
    private Inventory inventory;
    private Spell[] spells;             // Spell list

    // TODO ADD CURRENT GUN
    public Gun currentGun;

    void Start()
    {
        Debug.Log("Player start");
        this.Health = 100;
        this.Armor = 0;
        net = Vector3.zero;
        MovementSpeed = .1f;

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
            spells[i] = inventGameObj.transform.GetChild(1 + i).GetComponent<Spell>();
            spells[i].enabled = false; // initially disable spell use
        }
        // Init spell list
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
        switch_spell();
    }

    void move()
    {
        if (Input.GetKey("w"))
        {
            this.transform.position = this.transform.position + new Vector3(0, 0, MovementSpeed);
            net = net + new Vector3(0, 0, MovementSpeed);
        }
        if (Input.GetKey("s"))
        {
            this.transform.position = this.transform.position + new Vector3(0, 0, -MovementSpeed);
            net = net + new Vector3(0, 0, -MovementSpeed);
        }
        if (Input.GetKey("a"))
        {
            this.transform.position = this.transform.position + new Vector3(-MovementSpeed, 0, 0);
            net = net + new Vector3(-MovementSpeed, 0, 0);
        }
        if (Input.GetKey("d"))
        {
            this.transform.position = this.transform.position + new Vector3(MovementSpeed, 0, 0);
            net = net + new Vector3(MovementSpeed, 0, 0);
        }
        if (Input.GetKey("r"))
        {
            transform.GetChild(2).GetComponent<Gun>().Reload();
            sound.PlayOneShot(reload);
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

    void switch_spell()
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

    void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();
        GameObject itemObject = other.gameObject;

        GameObject weaponSlot = GameObject.FindGameObjectWithTag("currentWeapon");

        Debug.Log(weaponSlot.transform.name);

        if (item != null)
        {
            Debug.Log("Item picked up: " + item.name);
            inventory.AddItem(item);
            string name = item.name;

            item.isEquipped = true;
            item.transform.position = new Vector3(0.2f, 1, 0);
            item.transform.rotation = Quaternion.Euler(Vector3.zero);
            Instantiate(item, weaponSlot.transform);
            Debug.Log(this.transform.name);

            // item.OnPickup();
            Destroy(itemObject);
        }
    }
}
