using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For rendering other players when needed.
public class OtherPlayer : MonoBehaviour {

    public string displayName;
    public Transform playerPos;
    public float x_coordinate;
    public float y_coordinate;

    Rigidbody playerRigidbody;
    // Use this for initialization
    void Start () {
        x_coordinate = playerPos.transform.position.x;
        y_coordinate = playerPos.transform.position.y;
        playerRigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        x_coordinate = playerPos.transform.position.x;
        y_coordinate = playerPos.transform.position.y;
        playerRigidbody = GetComponent<Rigidbody>();
    }
}
