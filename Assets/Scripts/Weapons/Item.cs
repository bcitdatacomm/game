using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All item subclasses should call super.Update & super.OnTriggerEnter
public class Item : MonoBehaviour
{
    public int ID { get; set; }
    public byte Type;
    public float FireRate;
    public float Reload;
    public float ClipSize;

    void Update()
    {
        /* Rotates all item subclasses, looks pretty */
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }
}
