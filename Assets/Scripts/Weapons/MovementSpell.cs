using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpell : Spell
{

    ParticleSystem ps;

    void Start()
    {
        Debug.Log("Spell started");
        this.NumUses = 1;
        this.duration = 10;
        this.startTime = -1;
	GameObject obj = this.transform.parent.gameObject;
	Debug.Log(obj);
	ps = GetComponent<ParticleSystem>();
	var em = ps.emission;
	em.enabled = true;
    }

    void Update()
    {
        if(Input.GetButton("Fire2") && this.NumUses > 0)
        {
            Debug.Log("Spell used");
            --this.NumUses;
            this.Owner.MovementSpeed += 50;
            this.startTime = Time.time;
	    ps.Play();
        }


        if(this.startTime > 0)
        {
            if(Time.time - this.startTime >= this.duration)
            {
                this.startTime = 0;
                this.Owner.MovementSpeed -= 50;
		ps.Stop();
            }
        }


    }
}
