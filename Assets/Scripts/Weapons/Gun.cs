using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Bullet BulletPrefab;

    public Stack<Bullet> FiredShots;

    public float FireRate;

	public bool reloading;

    private float nextShotTime;

	private float bulletSpeed;

	private float bulletLifeTime;

	private int maxAmmo;
	private int currAmmo;

	private float reloadTime;



    void Start()
    {
        Debug.Log("Gun start");
        nextShotTime = 0;
        this.FiredShots = new Stack<Bullet>();
		this.FireRate = 0;
		this.bulletSpeed = .25f;
		this.bulletLifeTime = 1;
		this.maxAmmo = 10;
		this.reloadTime = 1;
		reloading = false;

		currAmmo = maxAmmo;
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
			if (currAmmo == 0)
			{
				reloading = true;
			}
        }

		if (reloading == true)
		{
			reloadTime = Time.time;
			if (reloadTime < 0)
			{
				currAmmo = maxAmmo;
			}
		}
    }
}
