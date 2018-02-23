using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All item subclasses should call super.Update & super.OnTriggerEnter
public class Item : MonoBehaviour {

	void Update ()
    {
        /* Rotates all item subclasses, looks pretty */
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime );    		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
