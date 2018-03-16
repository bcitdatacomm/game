using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : Item
{
    public Bullet BulletPrefab;

    public Stack<Bullet> FiredShots;

    //public float FireRate;

	public bool reloading;

    private float nextShotTime;

	private float bulletSpeed;

	private float bulletLifeTime;

	private int maxAmmo;
	private int currAmmo;

	private float reloadTime;
	private float timeBeforeReload;


    void Start()
    {
     Debug.Log("Gun start");
        nextShotTime = 0;
        this.FiredShots = new Stack<Bullet>();
		this.FireRate = 0; //0 is fastest, higher becomes slower
		this.bulletSpeed = .25f; //higher is faster
		this.bulletLifeTime = 1; //lifetime of bullet in seconds
		this.maxAmmo = 10; //max ammo of gun
		this.reloadTime = 2; //reload time in seconds.
		reloading = false;

		currAmmo = maxAmmo;
		transform.parent.GetChild (6).GetChild (1).GetChild (9).GetChild (0).GetComponent<SimpleHealthBar>().UpdateBar (currAmmo, maxAmmo);
    }

    void FixedUpdate()
    {
		if (Input.GetButton("Fire1") && Time.time > this.nextShotTime && currAmmo > 0 && reloading == false)
        {
			Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane plane=new Plane(Vector3.up, Vector3.zero);
			float distance;
			Vector3 direction = new Vector3(0,0,0);
			if(plane.Raycast(ray, out distance))
			{
				Vector3 target=ray.GetPoint(distance) + transform.parent.GetComponent<Player>().net;
				direction=target-transform.position;
				direction.y = 0;
				direction = direction.normalized;
			}

            Debug.Log("Shot Fired");
            this.nextShotTime = Time.time + this.FireRate;

			Bullet firedShot = (Bullet)Object.Instantiate(BulletPrefab, this.transform.position + direction, this.transform.rotation);

			firedShot.direction = direction;
			firedShot.Speed = bulletSpeed;
			firedShot.LifeTime = bulletLifeTime;

            if (firedShot != null)
            {
                this.FiredShots.Push(firedShot);
            }

			currAmmo--;
			transform.parent.GetChild (6).GetChild (1).GetChild (9).GetChild (0).GetComponent<SimpleHealthBar>().UpdateBar (currAmmo, maxAmmo);
			if (currAmmo <= 0)
			{
				Reload ();
			}
        }

		if (reloading == true)
		{	
			if (Time.time >= timeBeforeReload)
			{
				Debug.Log ("reloading done");
				currAmmo = maxAmmo;
				transform.parent.GetChild (6).GetChild (1).GetChild (9).GetChild (0).GetComponent<SimpleHealthBar>().UpdateBar (currAmmo, maxAmmo);
				reloading = false;
			}
		}
    }

	public void Reload()
	{
		timeBeforeReload = Time.time + reloadTime;
		reloading = true;
		Debug.Log ("reloading");
	}

}