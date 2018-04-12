using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	Inventory.cs
--
--	PROGRAM:		Player
--
--	FUNCTIONS:
--	class Inventory
--	{
--	         public void Start()
--                  public void AddItem()
--                  public byte getWeapon()
--	        public byte getSpell()
--                  public byte getCurrentSpell()
--	        byte[] getInventory()
--	}
--
--	class InventoryEventArgs
--	{
--		public InventoryEventArgs(Item item)
--	}
--
--	DATE:			Mar 20, 2018
--
--	REVISIONS:		Mar 21, 2018 refactored; consolidated spell functions
--
--	DESIGNERS:		Jeremy Lee, Li-Yan Tong
--
--	PROGRAMMER:	Jeremy Lee, Li-Yan Tong
--
--	NOTES:
--    	Keeps track of an individual player’s inventory.
---------------------------------------------------------------------------------------*/

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
    public Item[] items;

    /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: 		Start()
    --
    -- DATE: 			Mar 14, 2018
    --
    -- REVISIONS:		Mar 20, 2018 - Update to work with switching, addItem
    --				Mar 14, 2018 - Initialize values and comments
    --
    -- DESIGNER: 		Jeremy Lee, Li-Yan Tong
    --
    -- PROGRAMMER: 	Jeremy Lee, Li-Yan Tong
    --
    -- INTERFACE: 		Start()
    --
    -- RETURNS: 		void
    --
    -- NOTES:
    -- Initializes inventory to have capacity of four Items. Sets the current
    -- spell index in the inventory.
    -------------------------------------------------------------------------------------------------*/

    public void Start()
    {
        items = new Item[4];
        for (int i = 0; i < MAX_NUM_WEAPONS + MAX_NUM_SPELLS; i++)
        {
            items[i] = null;
        }
        CurrentSpell = SLOT_ITEM_SPELL;
    }
    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		AddItem()
        --
        -- DATE: 			Mar 20, 2018
        --
        -- REVISIONS:		Mar 20, 2018 - Update function based on events
        --				Mar 13, 2018 - Initialize function
        --
        -- DESIGNER: 		Jeremy Lee, Li-Yan Tong
        --
        -- PROGRAMMER: 	Jeremy Lee
        --
        -- INTERFACE: 		AddItem(Item item)
        --				item : the item to add to player inventory
        --
        -- RETURNS: 		void
        --
        -- NOTES:
        -- Adds a specified item to the inventory at the next available slot.
        -------------------------------------------------------------------------------------------------*/

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
    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		getWeapon()
       --
       -- DATE: 			Mar 20, 2018
       --
       -- REVISIONS:		Mar 20, 2018 - Initialize function
       --
       -- DESIGNER:		Benny Wang, Li-Yan Tong
       --
       -- PROGRAMMER: 	Li-Yan Tong
       --
       -- INTERFACE: 		getWeapon()
       --
       -- RETURNS: 		byte
       --
       -- NOTES:
       -- Returns weapon byte.
       -------------------------------------------------------------------------------------------------*/

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
    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		getSpell()
        --
        -- DATE: 			Mar 20, 2018
        --
        -- REVISIONS:		Mar 21, 2018
        --				- consolidated getSpell methods into one
        --
        -- DESIGNER: 		Jeremy Lee, Li-Yan Tong, Juliana French
        --
        -- PROGRAMMER: 	Jeremy Lee
        --
        -- INTERFACE: 		getSpell(int index)
        --				index : the index of the spell to return
        --
        -- RETURNS: 		byte
        --
        -- NOTES:
        -- Returns the specified spell byte.
        -------------------------------------------------------------------------------------------------*/

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
    /*-------------------------------------------------------------------------------------------------
      -- FUNCTION: 		getCurrentSpell()
      --
      -- DATE: 			Mar 20, 2018
      --
      -- REVISIONS:		Mar 21, 2018
      --
      --
      -- DESIGNER: 		Jeremy Lee, Li-Yan Tong
      --
      -- PROGRAMMER: 	Li-Yan Tong
      --
      -- INTERFACE: 		getCurrentSpell()
      --
      -- RETURNS: 		byte
      --
      -- NOTES:
      -- Returns the current spell byte.
      -------------------------------------------------------------------------------------------------*/

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
/*-------------------------------------------------------------------------------------------------
    -- FUNCTION: 		InventoryEventArgs(Item item)
    --
    -- DATE: 			Mar 20, 2018
    --
    -- REVISIONS:		Mar 20, 2018 - Initialize function
    --
    -- DESIGNER: 		Jeremy Lee, Li-Yan Tong
    --
    -- PROGRAMMER: 	Jeremy Lee
    --
    -- INTERFACE: 		InventoryEventArgs(Item item)
    -- 				item : the item to put in the inventory
    --
    -- RETURNS: 		constructed InventoryEventArgs
    --
    -- NOTES:
    -- Sets the given item as the Item.
    -------------------------------------------------------------------------------------------------*/

public class InventoryEventArgs : EventArgs
{
    public Item Item;

    public InventoryEventArgs(Item item)
    {
        Item = item;
    }
}
