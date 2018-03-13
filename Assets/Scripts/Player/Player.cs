using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Heath;

    public float MovementSpeed;

    public Gun PrimaryWeapon;

    public Spell[] Spells;

    public Stack<Bullet> FiredShots;

    void Start()
    {
        this.Heath = 100;

        // Change this to unarmed when it is made
        this.PrimaryWeapon = null;

        this.Spells = new Spell[3];

        this.FiredShots = new Stack<Bullet>();
    }

    void FixedUpdate()
    {
        this.move();
        this.turn();
        this.attack();
    }

    void move()
    {
        float deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * MovementSpeed;
        float deltaZ = Input.GetAxis("Vertical") * Time.deltaTime * MovementSpeed;
        this.transform.Translate(deltaX, 0, deltaZ);
    }

    void turn()
    {

    }

    void attack()
    {
        
    }
}
