using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public GameObject src;
    public GameController ClientController;
    public short BulletLifeTime;
    public uint PlayerId;
    public int BulletId;

    private void OnDestroy()
    {
        // ClientController.RemoveBullet(this);
    }

    void Start()
	{
        BulletId = GetInstanceID();
		Destroy(gameObject, (float)BulletLifeTime * 0.01f);
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
