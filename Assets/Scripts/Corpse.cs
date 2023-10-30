using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public InventoryHandler inventoryHandler;

    // Start is called before the first frame update
    public void SetVars(SpriteRenderer renderer, List<ItemHandler> items) {
        spriteRenderer.sprite = renderer.sprite;
        spriteRenderer.flipX = renderer.flipX;
        inventoryHandler.inventory = items;

        ObjectHandler owner = GetComponent<ObjectHandler>();

        foreach(ItemHandler item in inventoryHandler.inventory) {
            item.owner = owner;
        }
    }
}
