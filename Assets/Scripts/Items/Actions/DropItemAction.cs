using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drop Action", menuName = "Inventory/Actions/Drop Action")]
public class DropItemAction : BaseItemAction
{
    public override void Action(ItemAction action)
    {
        action.item.Drop();
    }
}
