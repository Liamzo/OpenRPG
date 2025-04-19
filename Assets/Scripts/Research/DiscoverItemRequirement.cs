using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DiscoverItemRequirement : DiscoverRequirement
{
    public string itemId;

    InventoryHandler inventoryHandler;


    public override string GetProgress() {
        return $"";
    }

    public override void Begin() {
        inventoryHandler = Player.Instance.GetComponent<InventoryHandler>();

        inventoryHandler.OnItemAdded += CheckInventory;
        inventoryHandler.OnItemRemoved += CheckInventory;
    }

    public override void End() {
        inventoryHandler.OnItemAdded -= CheckInventory;
        inventoryHandler.OnItemRemoved -= CheckInventory;
    }


    void CheckInventory(ItemHandler item) {
        current = 0;

        foreach (ItemHandler inventoryItem in inventoryHandler.inventory) {
            if (inventoryItem.objectHandler.prefabId == itemId)
                current++;
        }

        if (current >= total) {
            CallOnDiscover();
        }
    }
}
