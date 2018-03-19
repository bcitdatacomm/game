using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpell : Spell
{

    ParticleSystem ps;

    void Start()
    {
        Debug.Log("Spell started");
        duration = 10;
        startTime = -1;
        GameObject obj = this.transform.parent.gameObject;
        Debug.Log(obj);

        // For Sparkly fun time.
        ps = GetComponent<ParticleSystem>();
        var em = ps.emission;
        em.enabled = true;
    }

    void Update()
    {
        if (Input.GetButton("Fire2") && ClipSize > 0)
        {
            Debug.Log("Spell used");
            --ClipSize;
            transform.parent.GetComponent<Player>().MovementSpeed += 1;
            this.startTime = Time.time;
            ps.Play();
        }

        if (this.startTime > 0)
        {
            if (Time.time - this.startTime >= this.duration)
            {
                this.startTime = 0;
                transform.parent.GetComponent<Player>().MovementSpeed -= 1;
                ps.Stop();
            }
        }
    }
}
