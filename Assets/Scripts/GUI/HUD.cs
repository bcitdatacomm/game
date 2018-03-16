using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Inventory Inventory;

	// Use this for initialization
	void Start () {
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
                image.sprite = e.Item.Image;

                //TODO: Store reference to an item

                break;
            }
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
