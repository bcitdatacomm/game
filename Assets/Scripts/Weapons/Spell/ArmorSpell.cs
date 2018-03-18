using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSpell : Spell
{


    void Start()
    {
        Debug.Log("Spell started");
        this.NumUses = 1;
        this.duration = 0;
        this.startTime = 0;
    }

    void Update()
    {
        if(Input.GetButton("Fire2") && this.NumUses > 0)
        {
            Debug.Log("Spell used");
            --this.NumUses;
            this.Owner.Armor += 50;
        }


    }
}
