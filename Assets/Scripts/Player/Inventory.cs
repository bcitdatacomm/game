using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int ITEM_TYPE_WEAPON = 1;
    private const int ITEM_TYPE_SPELL = 2;
    private const int SLOT_ITEM_WEAPON = 0; // slot 1
    private const int SLOT_ITEM_SPELL = 1; // slot 2, ...
    private const int MAX_NUM_WEAPONS = 1;
    private const int MAX_NUM_SPELLS = 3;

    public int CurrentSpell { get; set; }
    public event EventHandler<InventoryEventArgs> ItemAdded;
    private Item[] items;

    public void Start()
    {
        items = new Item[4];
        for (int i = 0; i < MAX_NUM_WEAPONS + MAX_NUM_SPELLS; i++)
        {
            items[i] = null;
        }
        CurrentSpell = SLOT_ITEM_SPELL;
    }

    public void AddItem(Item item)
    {
        int slot = -1;

        if (item.Category == ITEM_TYPE_WEAPON)
        {
            slot = SLOT_ITEM_WEAPON;
        }
        else if (item.Category == ITEM_TYPE_SPELL)
        {
            // To add a spell to an empty spell slot
            for (int i = 0; i < MAX_NUM_SPELLS; i++)
            {
                if (items[i] == null)
                {
                    slot = SLOT_ITEM_WEAPON + i; // Spell slot starts after weapon
                    break;
                }
            }
            // To replace the current spell with the picked-up spell
            if (slot == -1)
            {
                slot = CurrentSpell;
            }
        }

        Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
        if (collider.enabled)
        {
            collider.enabled = false;
            items[slot] = item;
            // item.OnPickup();
        }

        if (slot >= 0) // Item is added: this logic doesn't really make sense
        {
            if (ItemAdded != null)
            {
                // Use this to notify the HUD to update
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }

    public byte getWeapon()
    {
        byte weapon;
        if (items[0] != null)
        {
            weapon = items[0].Type;
        }
        else
        {
            weapon = (byte)255;
        }
        return weapon;
    }

    public byte getSpell(int index)
    {
        byte spell;

        if (items[index] != null)
        {
            spell = items[index].Type;
        }
        else
        {
            spell = (byte)255;
        }
        return spell;
    }

    public byte getCurrentSpell()
    {
        byte currentSpell;
        if (items[CurrentSpell] != null)
        {
            currentSpell = items[CurrentSpell].Type;
        }
        else
        {
            currentSpell = (byte)255;
        }
        return currentSpell;
    }

}

public class InventoryEventArgs : EventArgs
{
    public Item Item;

    public InventoryEventArgs(Item item)
    {
        Item = item;
    }
}
