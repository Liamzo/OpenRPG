using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drop Action", menuName = "Inventory/Actions/Drop Action")]
public class DropItemAction : BaseItemAction
{
    public override void Action(ItemAction action)
    {
        action.item.owner.GetComponent<InventoryHandler>().Remove(action.item);
        action.item.transform.position = action.item.owner.transform.position;
        action.item.itemHitbox.enabled = true;
        action.item.gameObject.SetActive(true);
        action.item.owner = null;
    }
}
