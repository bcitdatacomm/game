using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int AoE;

    // Bullet Direction
    public Vector3 initPos;
    public Vector3 direction;

    void Start()
    {
        // Generate Identification, Direction and Timer to destroy bullet object
        this.ID = GetInstanceID();
        this.direction = this.direction.normalized;
        initPos = this.transform.position;
        Destroy(this.gameObject, LifeTime);
    }

    void FixedUpdate()
    {
        // Set Position of Bullet per Tick
        Vector3 tmp = direction;
        this.transform.position = this.transform.position + tmp * Speed;
        Vector3 p = this.transform.position;
        this.transform.position = new Vector3(p.x, 1, p.z);
    }
}
