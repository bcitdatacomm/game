using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        Debug.Log("Gun start");
        nextShotTime = 0;
        reloading = false;

        // Get audio
        weaponSound = GetComponent<AudioSource>();

        // Get from Prefab.
        currAmmo = ClipSize;
    }

    void FixedUpdate()
    {
        // Only allow Equipped guns to shoot; If this check is gone all guns shoot! Logic fix required...
        //if (this.isEquipped)
        //{
        Shoot();
        ReloadCheck();
        //}
    }

    /*
     * Called on Cooldown and manual player reload
     */
    public void Reload()
    {
        timeBeforeReload = Time.time + reloadTime;
        reloading = true;
        Debug.Log("reloading");
    }

    void ReloadCheck()
    {
        if (reloading == true)
        {
            if (Time.time >= timeBeforeReload)
            {
                Debug.Log("reloading done");
                currAmmo = ClipSize;
                reloading = false;
            }
        }
    }

    void Shoot()
    {
        if (Input.GetButton("Fire1") && Time.time > this.nextShotTime && currAmmo > 0 && reloading == false)
        {
            Debug.Log("Shot Fired");
            weaponSound.Play();
            this.nextShotTime = Time.time + this.FireRate;

            // Create Bullet at Parent position (player)
            Bullet firedShot = (Bullet)GameObject.Instantiate(BulletPrefab, this.transform.parent.position, this.transform.parent.rotation);
            // Rotate bullet and multiply by parent forward direction
            firedShot.direction = this.GetComponentInParent<Transform>().rotation * Vector3.forward;

            if (firedShot != null)
            {
                // Debug.Log("GUN: Bullet ID ShotAgain: " + firedShot.GetInstanceID());
                GameObject PlayerRef = GameObject.FindGameObjectWithTag("Player");
                Player player = PlayerRef.GetComponent<Player>();
                player.FiredShots.Push(firedShot);
                player.TrackedShots.Add(firedShot.GetInstanceID(), firedShot);
                // Debug.Log("GUN: Bullet Dictionary" + player.TrackedShots.ContainsKey(firedShot.ID));
            }

            currAmmo--;

            if (currAmmo <= 0)
            {
                Reload();
            }
        }
    }
}
