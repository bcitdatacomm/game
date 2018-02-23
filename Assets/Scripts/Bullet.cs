using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        //var health = hit.GetComponent<Health>();
        var health = hit.health;
        if (health != null)
        {
            health -= 10;
        }

        Destroy(gameObject);
    }
}
