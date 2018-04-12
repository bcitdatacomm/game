using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	HUD.cs
--
--	PROGRAM:		GUI
--
--	FUNCTIONS:
--				void Start()
--				public void InventoryScript_ItemAdded()
--				void Update()
--				void updateLife()
--				void updateAmmoHud()
--
--	DATE:			Mar 20, 2018
--
--	REVISIONS:		Mar 21, 2018
--
--	DESIGNERS:		John Tee
--
--	PROGRAMMER:	John Tee
--
--	NOTES:
--    Defines behaviour for the heads up display in game.
---------------------------------------------------------------------------------------*/

public class HUD : MonoBehaviour {

    public Inventory Inventory;



    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		Start()
        --
        -- DATE: 			Mar 20, 2018
        --
        -- REVISIONS:		Mar 21, 2018 - start method now emptied
        --
        -- DESIGNER: 		John Tee
        --
        -- PROGRAMMER: 	John Tee
        --
        -- INTERFACE: 		Start()
        --
        -- RETURNS: 		void
        --
        -- NOTES:
        -- Initializes HUD.
        -------------------------------------------------------------------------------------------------*/

	void Start ()
    {
        // Inventory.ItemAdded += InventoryScript_ItemAdded;

	}
  /*-------------------------------------------------------------------------------------------------
     -- FUNCTION: 		InventoryScript_ItemAdded()
     --
     -- DATE: 			Mar 20, 2018
     --
     -- REVISIONS:
     --
     -- DESIGNER: 		John Tee
     --
     -- PROGRAMMER: 	John Tee
     --
     -- INTERFACE: 		InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
     --				sender : the object that sent the event
     --				e : the sent event
     --
     -- RETURNS: 		void
     --
     -- NOTES:
     -- Stores a new inventory item as an image in the HUD inventory panel.
     -------------------------------------------------------------------------------------------------*/

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        foreach(Transform Slot in inventoryPanel)
        {
            Image image = Slot.GetChild(0).GetChild(0).GetComponent<Image>();

            if(!image.enabled)
            {
                image.enabled = true;
                //image.sprite = e.Item.Image;

                //TODO: Store reference to an item

                break;
            }
        }
    }

    /*-------------------------------------------------------------------------------------------------
       -- FUNCTION: 		Update()
       --
       -- DATE: 			Mar 20, 2018
       --
       -- REVISIONS:		Mar 21, 2018
       --
       -- DESIGNER: 		John Tee
       --
       -- PROGRAMMER: 	John Tee
       --
       -- INTERFACE: 		Update()
       --
       -- RETURNS: 		void
       --
       -- NOTES:
       -- Main method that updates player health and ammo.
       -------------------------------------------------------------------------------------------------*/

	void Update ()
    {
		updateLife();
        updateAmmoHud();
	}
  /*-------------------------------------------------------------------------------------------------
     -- FUNCTION: 		updateLife()
     --
     -- DATE: 			Mar 21, 2018
     --
     -- REVISIONS:
     --
     -- DESIGNER: 		John Tee
     --
     -- PROGRAMMER: 	John Tee
     --
     -- INTERFACE: 		updateLife()
     --
     -- RETURNS: 		void
     --
     -- NOTES:
     -- Updates the current player health and armor.
     -------------------------------------------------------------------------------------------------*/

	void updateLife()
	{
		int hudHp = this.transform.parent.GetComponent<Player>().Health;
		int hudArmor = this.transform.parent.GetComponent<Player>().Armor;
		this.transform.Find("Vitals").Find("VerticalLayout").Find("Life").Find("Health").Find("CurrentHealth").GetComponent<SimpleHealthBar>().UpdateBar(hudHp, 100);
		this.transform.Find("Vitals").Find("VerticalLayout").Find("Life").Find("Armor").Find("CurrentArmor").GetComponent<SimpleHealthBar>().UpdateBar(hudArmor, 100);
	}
  /*-------------------------------------------------------------------------------------------------
     -- FUNCTION: 		updateAmmoHud()
     --
     -- DATE: 			Mar 21, 2018
     --
     -- REVISIONS:
     --
     -- DESIGNER: 		John Tee
     --
     -- PROGRAMMER: 	John Tee
     --
     -- INTERFACE: 		updateAmmoHud()
     --
     -- RETURNS: 		void
     --
     -- NOTES:
     -- Updates whether the player is able to reload, checks the clip size, and
     -- updates the current ammo available to the player.
     -------------------------------------------------------------------------------------------------*/

    void updateAmmoHud()
    {
		if (this.transform.parent.Find ("Inventory").Find ("Weapon").childCount > 0) {
			bool hudReload = this.transform.parent.Find ("Inventory").Find ("Weapon").GetChild (0).GetComponent<Gun> ().reloading;
			if (hudReload == false) {
				int hudCurrAmmo = this.transform.parent.Find ("Inventory").Find ("Weapon").GetChild (0).GetComponent<Gun> ().currAmmo;
				int hudClipSize = this.transform.parent.Find ("Inventory").Find ("Weapon").GetChild (0).GetComponent<Gun> ().ClipSize;
				this.transform.Find("Vitals").Find("VerticalLayout").Find ("Weapons").Find ("AmmoBar").Find ("CurrentAmmo").GetComponent<SimpleHealthBar> ().UpdateBar (hudCurrAmmo, hudClipSize);
                this.transform.Find("Vitals").Find("VerticalLayout").Find ("Weapons").Find ("AmmoBar").Find ("AmmoText").GetComponent<Text> ().text = "" + hudCurrAmmo + "/" + hudClipSize;
			}
			else
			{
				this.transform.Find("Vitals").Find("VerticalLayout").Find ("Weapons").Find ("AmmoBar").Find ("CurrentAmmo").GetComponent<SimpleHealthBar> ().UpdateBar (0, 0);
			}
		}

    }
}
