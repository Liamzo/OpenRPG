using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AssembleRequirement : ResearchRequirement
{
    public string itemId;

    InventoryHandler inventoryHandler;


    public override string GetProgress() {
        return $"{current}/{total} {PrefabManager.Instance.FindBaseStatsById(itemId).objectName}";
    }

    public override void Begin() {
        inventoryHandler = Player.Instance.GetComponent<InventoryHandler>();

        inventoryHandler.OnItemAdded += CheckInventory;
        inventoryHandler.OnItemRemoved += CheckInventory;
    }

    public override void End() {
        inventoryHandler.OnItemAdded -= CheckInventory;
        inventoryHandler.OnItemRemoved -= CheckInventory;

        for (int i = 0; i < total; i++) {
            foreach (ItemHandler inventoryItem in inventoryHandler.inventory) {
                if (inventoryItem.objectHandler.prefabId == itemId) {
                    inventoryHandler.Remove(inventoryItem);
                    break;
                }
            }
        } 
    }


    void CheckInventory(ItemHandler item) {
        current = 0;

        foreach (ItemHandler inventoryItem in inventoryHandler.inventory) {
            if (inventoryItem.objectHandler.prefabId == itemId)
                current++;
        }
    }
}
