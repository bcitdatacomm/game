using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : Item
{
    // Link Via unity
    public Bullet BulletPrefab;
    // Keep Track of Bullets
    public Stack<Bullet> FiredShots;
    // Check if can reload
    public bool reloading;
    // Time in seconds between shots
    private float nextShotTime;
    // Available bullets
    public int currAmmo;
    // Time in seconds for a Reload
    private float timeBeforeReload;

    void Start()
    {
        Debug.Log("Gun start");
        nextShotTime = 0;
        this.FiredShots = new Stack<Bullet>();
        reloading = false;

        // Get from Prefab.
        currAmmo = ClipSize;
    }

    void FixedUpdate()
    {
        // Only allow Equipped guns to shoot; If this check is gone all guns shoot!
        if (this.isEquipped)
        {
            Shoot();
            ReloadCheck();
            // TODO: Need to move this to HUD
            //this.transform.parent.Find("HUD").Find("Weapons").Find("AmmoBar").Find("CurrentAmmo").GetComponent<SimpleHealthBar>().UpdateBar(currAmmo, ClipSize);
        }
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
                // TODO: MOVE TO HUD
                //transform.parent.Find("HUD").Find("Weapons").Find("AmmoBar").Find("CurrentAmmo").GetComponent<SimpleHealthBar>().UpdateBar (currAmmo, ClipSize);
                reloading = false;
            }
        }
    }

    void Shoot()
    {
        if (Input.GetButton("Fire1") && Time.time > this.nextShotTime && currAmmo > 0 && reloading == false)
        {
            Debug.Log("Shot Fired");
            this.nextShotTime = Time.time + this.FireRate;

            // Create Bullet at Parent position (player)
            Bullet firedShot = (Bullet)Object.Instantiate(BulletPrefab, this.transform.parent.position, this.transform.parent.rotation);
            // Rotate bullet and multiply by parent forward direction
            firedShot.direction = this.GetComponentInParent<Transform>().rotation * Vector3.forward;

            if (firedShot != null)
            {
                this.FiredShots.Push(firedShot);
            }

            currAmmo--;

            // TODO: MOVE TO HUD
            // transform.parent.Find("HUD").Find("Weapons").Find("AmmoBar").Find("CurrentAmmo").GetComponent<SimpleHealthBar>().UpdateBar(currAmmo, ClipSize);
            if (currAmmo <= 0)
            {
                Reload();
            }
        }
    }
}
