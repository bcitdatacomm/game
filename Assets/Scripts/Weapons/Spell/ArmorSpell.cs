using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSpell : Spell
{


    void Start()
    {
        Debug.Log("Spell started");
    }

    void Update()
    {
        if (Input.GetButton("Fire2") && ClipSize > 0)
        {
            Debug.Log("Spell used");
            --ClipSize;
            transform.parent.GetComponent<Player>().Armor += 50;
        }
    }
}
