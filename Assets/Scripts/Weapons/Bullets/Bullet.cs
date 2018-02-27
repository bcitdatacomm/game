using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public GameObject src;
    public short bulletLifeTime; 
    public uint playerId; 

	void Start()
	{
		Destroy(gameObject, (float)bulletLifeTime * 0.01f);
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
        	target.Health -= 10;
	        Debug.Log("target hp:" + target.Health);    		
        }
        Destroy(gameObject);
    }
}