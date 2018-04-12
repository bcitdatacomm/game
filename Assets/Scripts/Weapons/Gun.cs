using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	Gun.cs
--
--	PROGRAM:		Weapon
--
--	FUNCTIONS:
--				void Start()
--				void FixedUpdate()
--				public void Reload()
--				void ReloadCheck()
--				void Shoot()
--
--	DATE:			Mar 17, 2018
--
--	REVISIONS:		Mar 20, 2018
--				Apr 10, 2018 - refactor, clearer naming
--
--	DESIGNERS:		Li-Yan Tong, John Tee
--
--	PROGRAMMER:	John Tee, Benny Wang
--
--	NOTES:
--    Defines gun behaviour for a player.
---------------------------------------------------------------------------------------*/

public class Gun : Item
{
    // Link Via unity
    public Bullet BulletPrefab;
    // Check if can reload
    public bool reloading;
    // Time in seconds between shots
    private float nextShotTime;
    // Available bullets
    public int currAmmo;
    // Time in seconds for a Reload
    private float timeBeforeReload;
    // weapon audio
    public AudioSource weaponSound;
    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		Start()
       --
       -- DATE: 			Mar 17, 2018
       --
       -- REVISIONS:		Mar 20, 2018 - added sound, clip size
       --				Apr 10, 2018 - refactor, clearer naming
       --
       -- DESIGNER: 		Li-Yan Tong, John Tee
       --
       -- PROGRAMMER: 	John Tee, Anthony Vu
       --
       -- INTERFACE: 		Start()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Instatiates a gun with no shots fired.
       -------------------------------------------------------------------------------------------------*/

    void Start()
    {
        Debug.Log("Gun start");
        this.nextShotTime = 0;
        this.reloading = false;

        // Get audio
        this.weaponSound = GetComponent<AudioSource>();

        // Get from Prefab.
        this.currAmmo = ClipSize;
    }

    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		Reload()
       --
       -- DATE: 			Mar 20, 2018
       --
       -- REVISIONS:
       --
       -- DESIGNER: 		John Tee, Jeffrey Chou
       --
       -- PROGRAMMER: 	John Tee
       --
       -- INTERFACE: 		Reload()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Allows reloading of ammo to the gun.
       -------------------------------------------------------------------------------------------------*/

    public void Reload()
    {
        this.timeBeforeReload = Time.time + reloadTime;
        this.reloading = true;
    }

     /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		CheckReload()
        --
        -- DATE: 			Mar 20, 2018
        --
        -- REVISIONS:		Apr 10, 2018 - refactor, clearer naming
        --
        -- DESIGNER: 		John Tee, Jeffrey Chou
        --
        -- PROGRAMMER: 	John Tee
        --
        -- INTERFACE: 		CheckReload()
        --
        -- RETURNS: 		void
        --
        -- NOTES:
        -- Checks if the player has reloaded ammo, and then actually refills
        -- the player’s ammo.
        -------------------------------------------------------------------------------------------------*/

    public void CheckReload()
    {
        if (this.reloading == true)
        {
            if (Time.time >= this.timeBeforeReload)
            {
                this.currAmmo = ClipSize;
                this.reloading = false;
            }
        }
    }
    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		CheckShoot()
       --
       -- DATE: 			Mar 20, 2018
       --
       -- REVISIONS:		Mar 21, 2018 - now tracks shots fired
       --				Apr 10, 2018 - refactor, clearer naming
       --
       -- DESIGNER: 		Li-Yan Tong, Juliana French
       --
       -- PROGRAMMER: 	Li-Yan Tong
       --
       -- INTERFACE: 		CheckShoot()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Allows player to shoot their gun if they have ammo and are not reloading.
       -------------------------------------------------------------------------------------------------*/

    public void CheckShoot()
    {
        if (Input.GetButton("Fire1") && Time.time > this.nextShotTime && currAmmo > 0 && reloading == false)
        {
            this.shoot();
        }
    }
    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		shoot()
       --
       -- DATE: 			Mar 20, 2018
       --
       -- REVISIONS:		Mar 21, 2018 - now tracks shots fired
       --				Apr 10, 2018 - refactor, clearer naming
       --
       -- DESIGNER: 		Benny Wang, Li-Yan Tong
       --
       -- PROGRAMMER: 	Benny Wang
       --
       -- INTERFACE: 		CheckShoot()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Allows player to shoot their gun with audio, determines fire rate.
       -------------------------------------------------------------------------------------------------*/

    void shoot()
    {
        GameObject PlayerRef = GameObject.FindGameObjectWithTag("Player");
        Player player = PlayerRef.GetComponent<Player>();

        weaponSound.Play();
        this.nextShotTime = Time.time + this.FireRate;

        // Create Bullet at Parent position (player)
        Bullet firedShot = (Bullet)GameObject.Instantiate(BulletPrefab, this.transform.parent.position, this.transform.parent.rotation);
        firedShot.ID = firedShot.GetInstanceID();
        // Rotate bullet and multiply by parent forward direction
        firedShot.direction = this.GetComponentInParent<Transform>().rotation * Vector3.forward;

        if (firedShot != null)
        {
            player.FiredShots.Push(firedShot);
            player.TrackedShots.Add(firedShot.ID, firedShot);
            Debug.Log("Player fired bullet with id " + firedShot.ID);
        }

        currAmmo--;

        if (currAmmo <= 0)
        {
            Reload();
            player.sound.PlayOneShot(player.reload);
        }
    }
}
