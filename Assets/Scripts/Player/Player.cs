using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Heath;

    public float MovementSpeed;

    public Spell[] Spells;

    void Start()
    {
        Debug.Log("Player start");
        this.Heath = 100;
        this.Spells = new Spell[3];
    }

    void FixedUpdate()
    {
        this.move();
        this.turn();
    }

    void move()
    {
        float deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * MovementSpeed;
        float deltaZ = Input.GetAxis("Vertical") * Time.deltaTime * MovementSpeed;
        this.transform.Translate(deltaX, 1, deltaZ);
    }

    void turn()
    {

    }
}
