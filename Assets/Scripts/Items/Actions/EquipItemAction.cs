using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equip Action", menuName = "Inventory/Actions/Equip Action")]
public class EquipItemAction : BaseItemAction
{
    public override void Action(ItemAction action)
    {
        object slot;
        if (action.data.TryGetValue(ItemActionDataType.EquipSlot, out slot)) {
            action.item.owner.GetComponent<EquipmentHandler>().Equip(action.item, (EquipmentSlot)slot);
        } else {
            action.item.owner.GetComponent<EquipmentHandler>().Equip(action.item, action.item.baseItemStats.possibleSlots[0]);
        }
    }

    public override string MakeMenuName(ItemAction action)
    {
        object slot;
        if (action.data.TryGetValue(ItemActionDataType.EquipSlot, out slot)) {
            return menuName + " " + System.Text.RegularExpressions.Regex.Replace(((EquipmentSlot)slot).ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        } else {
            return menuName;
        }
    }
}
