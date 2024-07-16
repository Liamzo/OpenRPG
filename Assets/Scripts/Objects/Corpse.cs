using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    public BaseStats baseStats;
    public SpriteRenderer spriteRenderer;
    public InventoryHandler inventoryHandler;


    public void SetVars(SpriteRenderer renderer, List<ItemHandler> items) {
        ObjectHandler owner = GetComponent<ObjectHandler>();
        owner.CreateBaseObject(baseStats);

        inventoryHandler.inventory = items;
        foreach(ItemHandler item in inventoryHandler.inventory) {
            item.owner = owner;
            item.transform.parent = transform;
        }

        spriteRenderer.sprite = renderer.sprite;
        spriteRenderer.flipX = renderer.flipX;
    }
}
