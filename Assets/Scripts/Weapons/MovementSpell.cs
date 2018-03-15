using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpell : Spell
{

    void Start()
    {
        Debug.Log("Spell started");
        this.NumUses = 1;
        this.duration = 10;
        this.startTime = -1;
    }

    void Update()
    {
        if(Input.GetButton("Fire2") && this.NumUses > 0)
        {
            Debug.Log("Spell used");
            --this.NumUses;
            this.Owner.MovementSpeed += 50;
            this.startTime = Time.time;
        }


        if(this.startTime > 0)
        {
            if(Time.time - this.startTime >= this.duration)
            {
                this.startTime = 0;
                this.Owner.MovementSpeed -= 50;
            }
        }


    }
}
