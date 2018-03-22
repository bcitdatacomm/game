using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Inventory Inventory;



	// Use this for initialization
	void Start ()
    {
        Inventory.ItemAdded += InventoryScript_ItemAdded;

	}

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

	// Update is called once per frame
	void Update ()
    {
		updateLife();
        updateAmmoHud();
	}

	void updateLife()
	{
		int hudHp = this.transform.parent.GetComponent<Player>().Health;
		int hudArmor = this.transform.parent.GetComponent<Player>().Armor;
		this.transform.Find("Life").Find("Health").Find("CurrentHealth").GetComponent<SimpleHealthBar>().UpdateBar(hudHp, 100);
		this.transform.Find("Life").Find("Armor").Find("CurrentArmor").GetComponent<SimpleHealthBar>().UpdateBar(hudArmor, 100);
	}

    void updateAmmoHud()
    {	
		if (this.transform.parent.Find ("Inventory").Find ("Weapon").childCount > 0) {
			bool hudReload = this.transform.parent.Find ("Inventory").Find ("Weapon").GetChild (0).GetComponent<Gun> ().reloading;
			if (hudReload == false) {
				int hudCurrAmmo = this.transform.parent.Find ("Inventory").Find ("Weapon").GetChild (0).GetComponent<Gun> ().currAmmo;
				int hudClipSize = this.transform.parent.Find ("Inventory").Find ("Weapon").GetChild (0).GetComponent<Gun> ().ClipSize;
				this.transform.Find ("Weapons").Find ("AmmoBar").Find ("CurrentAmmo").GetComponent<SimpleHealthBar> ().UpdateBar (hudCurrAmmo, hudClipSize);
			} 
			else 
			{
				this.transform.Find ("Weapons").Find ("AmmoBar").Find ("CurrentAmmo").GetComponent<SimpleHealthBar> ().UpdateBar (0, 0);
			}
		}
			
    }
}
