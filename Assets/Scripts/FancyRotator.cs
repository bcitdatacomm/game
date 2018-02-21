using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyRotator : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
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
