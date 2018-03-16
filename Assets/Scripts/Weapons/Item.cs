using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All item subclasses should call super.Update & super.OnTriggerEnter
public class Item : MonoBehaviour
{
    public int ID { get; set; }
    public byte Type;
    public float FireRate;
    public int ClipSize;
	public float reloadTime; //reload time in seconds.

}
