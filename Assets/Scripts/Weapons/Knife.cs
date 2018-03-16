using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Item, IInventoryItem {

    public string Name
    {
        get
        {
            return "Knife";
        }
    }

    public Sprite _Image = null;

    public Sprite Image
    {
        get
        {
            return _Image;
        }
    }

    public void OnPickup()
    {
        // TODO: Add logic to put only in weapon slot and no bullets.
        gameObject.SetActive(false);
    }
}
