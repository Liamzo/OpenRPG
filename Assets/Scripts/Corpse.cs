using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public InventoryHandler inventoryHandler;

    // Start is called before the first frame update
    public void SetVars(Sprite sprite, List<ItemHandler> items) {
        spriteRenderer.sprite = sprite;
        inventoryHandler.inventory = items;

        ObjectHandler owner = GetComponent<ObjectHandler>();

        foreach(ItemHandler item in inventoryHandler.inventory) {
            item.owner = owner;
        }
    }
}
