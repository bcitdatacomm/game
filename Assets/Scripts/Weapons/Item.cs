using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	Gun.cs
--
--	PROGRAM:		Weapon
--
--	FUNCTIONS:
--
--	DATE:			Mar 20, 2018
--
--	REVISIONS:
--
--	DESIGNERS:		Benny Wang, Jeremy Lee, Li-Yan Tong
--
--	PROGRAMMER:	Jeremy Lee, Li-Yan Tong
--
--	NOTES:
--    Defines an item’s member variables.
---------------------------------------------------------------------------------------*/

public class Item : MonoBehaviour
{
    public int ID { get; set; }
    public int Category;
    public byte Type;
    public float FireRate;
    public float reloadTime; //reload time in seconds.
    public int ClipSize;
    public bool isEquipped;
}
