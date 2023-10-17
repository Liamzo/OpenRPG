using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unequip Action", menuName = "Inventory/Actions/Unequip Action")]
public class UnequipItemAction : BaseItemAction
{
    public override void Action(ItemAction action)
    {
        action.item.owner.GetComponent<EquipmentHandler>().Unequip(action.item);
    }
}
