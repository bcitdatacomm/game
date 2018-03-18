using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStackTest : MonoBehaviour {

    public GameObject player;

	// Update is called once per frame
	void Update ()
    {
        Gun gun = player.transform.Find("Inventory").Find("Weapon").GetChild(0).GetComponent<Gun>();

        if (gun != null)
        {
            Debug.Log("gun found");

            Stack<Bullet> playerBullets = gun.FiredShots;

            while (playerBullets.Count > 0)
            {
                Debug.Log("removing " + playerBullets.Pop().ID);
            }
        }
    }
}
