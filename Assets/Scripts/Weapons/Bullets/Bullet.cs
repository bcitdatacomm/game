using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour
{
    // Environment ID
    public int ID { get; set; }

    // Defined per Prefab
    // Packet Identification
    public byte Type;
    // Decrement Health
    public int Damage;
    // Projectile Speed
    public float Speed;
    // Seconds till Bullet Destroyed
    public float LifeTime;

    // Bullet Direction
    public Vector3 initPos;
    public Vector3 direction;

    void Start()
    {
        // Generate Identification, Direction and Timer to destroy bullet object
        this.direction = this.direction.normalized;
        this.initPos = this.transform.position;

        // Server should do this for us
        // Destroy(this.gameObject, LifeTime);
        Debug.Log("Bullet spawned with id: " + this.ID);
    }

    void FixedUpdate()
    {
        // Set Position of Bullet per Tick
        Vector3 tmp = direction;
        this.transform.position = this.transform.position + tmp * Speed;
        Vector3 p = this.transform.position;
        this.transform.position = new Vector3(p.x, 1, p.z);
    }

    public byte[] ToBytes() 
    {
        byte[] temp = new byte[5];
        Buffer.BlockCopy(BitConverter.GetBytes(this.ID), 0, temp, 0, 4);
        temp[4] = this.Type;
        return temp;
    }
}
