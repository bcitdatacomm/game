using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : Item
{
    public Bullet BulletPrefab;

    public Stack<Bullet> FiredShots;

	private bool reloading;

    private float nextShotTime;

	private int currAmmo;

	private float timeBeforeReload; 


    void Start()
    {
     Debug.Log("Gun start");
        nextShotTime = 0;
        this.FiredShots = new Stack<Bullet>();
		reloading = false;

		currAmmo = ClipSize;
		//transform.parent.Find("HUD").Find("Weapons").Find("AmmoBar").Find("CurrentAmmo").GetComponent<SimpleHealthBar>().UpdateBar (currAmmo, ClipSize);
    }

    void FixedUpdate()
    {
		Shoot ();
		ReloadCheck();
    }

	public void Reload()
	{
		timeBeforeReload = Time.time + reloadTime;
		reloading = true;
		Debug.Log ("reloading");
	}

	void ReloadCheck()
	{
		if (reloading == true)
		{	
			if (Time.time >= timeBeforeReload)
			{
				Debug.Log ("reloading done");
				currAmmo = ClipSize;
				transform.parent.Find("HUD").Find("Weapons").Find("AmmoBar").Find("CurrentAmmo").GetComponent<SimpleHealthBar>().UpdateBar (currAmmo, ClipSize);
				reloading = false;
			}
		}
	}

	void Shoot()
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
		

			if (firedShot != null)
			{
				this.FiredShots.Push(firedShot);
			}

			currAmmo--;
			transform.parent.Find("HUD").Find("Weapons").Find("AmmoBar").Find("CurrentAmmo").GetComponent<SimpleHealthBar>().UpdateBar (currAmmo, ClipSize);
			if (currAmmo <= 0)
			{
				Reload ();
			}
		}
	}
}