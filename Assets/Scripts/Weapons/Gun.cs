using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    public Bullet BulletPrefab;

    public Stack<Bullet> FiredShots;

    //public float FireRate;

    private float nextShotTime;

    void Start()
    {
        Debug.Log("Gun start");
        nextShotTime = 0;
        this.FiredShots = new Stack<Bullet>();
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Fire1") && Time.time > this.nextShotTime)
        {
            Debug.Log("Shot Fired");
            this.nextShotTime = Time.time + this.FireRate;

            Bullet firedShot = (Bullet)Object.Instantiate(BulletPrefab, this.transform.position, this.transform.rotation);

            if (firedShot != null)
            {
                this.FiredShots.Push(firedShot);
            }
        }
    }
}
