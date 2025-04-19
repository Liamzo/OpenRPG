using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class DiscoverModRequirement : DiscoverRequirement
{
    public string modId;

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


    // This is probably inefficient, but easiest way to not miss anything
    void CheckInventory(ItemHandler item) {
        current = 0;

        foreach (ItemHandler inventoryItem in inventoryHandler.inventory) {
            if (inventoryItem.TryGetComponent<WeaponHandler>(out WeaponHandler weapon)) {
                foreach (WeaponMod mod in weapon.mods.Values) {
                    if (mod != null) {
                        if (mod.modId == modId)
                            current++;
                    }
                }
            }
        }

        if (current >= total) {
            CallOnDiscover();
        }
    }
}
