using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Public variable to store a reference to the player game object

    public GameObject Player;

    //Private variable to store the offset distance between the player and camera
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        //offset = transform.position - Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Player.transform.position + offset;
    }
}
