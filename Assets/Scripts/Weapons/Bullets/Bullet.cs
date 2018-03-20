using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int ID { get; set; }     // Environment ID
    public byte Type;               // Packet Identification
    public int Damage;              // Decrement Health
    public float Speed;             // Projectile Speed
    public float LifeTime;          // Seconds till Bullet Destroyed
    public int AoE;
    public Vector3 initPos;
	public Vector3 direction;       // Direction "forward" from parent player

    void Start()
    {
        // Generate Identification, Direction and Timer to destroy bullet object
        this.ID = GetInstanceID();
        this.direction = this.direction.normalized;
        initPos = this.transform.position;
		Destroy (this.gameObject, LifeTime);
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
