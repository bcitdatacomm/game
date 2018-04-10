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
        this.nextShotTime = 0;
        this.reloading = false;

        // Get audio
        this.weaponSound = GetComponent<AudioSource>();

        // Get from Prefab.
        this.currAmmo = ClipSize;
    }

    /*
     * Called on Cooldown and manual player reload
     */
    public void Reload()
    {
        this.timeBeforeReload = Time.time + reloadTime;
        this.reloading = true;
    }

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

    public void CheckShoot()
    {
        if (Input.GetButton("Fire1") && Time.time > this.nextShotTime && currAmmo > 0 && reloading == false)
        {
            this.shoot();
        }
    }

    void shoot()
    {
        GameObject PlayerRef = GameObject.FindGameObjectWithTag("Player");
        Player player = PlayerRef.GetComponent<Player>();

        weaponSound.Play();
        this.nextShotTime = Time.time + this.FireRate;

        // Create Bullet at Parent position (player)
        Bullet firedShot = (Bullet)GameObject.Instantiate(BulletPrefab, this.transform.parent.position, this.transform.parent.rotation);
        // Rotate bullet and multiply by parent forward direction
        firedShot.direction = this.GetComponentInParent<Transform>().rotation * Vector3.forward;
        
        if (firedShot != null)
        {
            player.FiredShots.Push(firedShot);
            player.TrackedShots.Add(firedShot.GetInstanceID(), firedShot);
        }

        currAmmo--;

        if (currAmmo <= 0)
        {
            Reload();
            player.sound.PlayOneShot(player.reload);
        }
    }
}
