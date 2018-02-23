using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public GameObject src;
    public float bulletLifeTime = 1.0f; 

	void Start()
	{
		Destroy(gameObject, bulletLifeTime);
	}

    void OnCollisionEnter(Collision collision)
    {
    	if (collision.gameObject == src)
    	{
    		return;
    	}
        var hit = collision.gameObject;
        var target = hit.GetComponent<Player>();
	    Debug.Log("bullet collided:" + hit);    		

        if (target != null)
        {
        	target.health -= 10;
	        Debug.Log("target hp:" + target.health);    		
        }
        Destroy(gameObject);
    }
}