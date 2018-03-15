using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int ID { get; set; }
    public byte Type;
    public int Damage;
    public float Speed;
    public float LifeTime;
    public int AoE;
	public Vector3 direction;

    void Start()
    {
        this.ID = GetInstanceID();
    }

    void FixedUpdate()
    {
		this.transform.position = this.transform.position + direction * Speed;
    }
}
