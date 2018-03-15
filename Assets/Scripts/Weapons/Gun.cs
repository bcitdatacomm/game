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
			Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane plane=new Plane(Vector3.up, Vector3.zero);
			float distance;
			Vector3 direction = new Vector3(0,0,0);
			if(plane.Raycast(ray, out distance))
			{
				Vector3 target=ray.GetPoint(distance);
				direction=target-transform.position;
				direction.y = 0;
				direction = direction.normalized;
			}

            Debug.Log("Shot Fired");
            this.nextShotTime = Time.time + this.FireRate;

			Bullet firedShot = (Bullet)Object.Instantiate(BulletPrefab, this.transform.position + direction, this.transform.rotation);
			firedShot.direction = direction;

			firedShot.Speed = 0.25f;

            if (firedShot != null)
            {
                this.FiredShots.Push(firedShot);
            }
        }
    }
}
