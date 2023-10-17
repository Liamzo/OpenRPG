using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Look Action", menuName = "Inventory/Actions/Look Action")]
public class LookItemAction : BaseItemAction
{
    public override void Action(ItemAction action)
    {
        Debug.Log(action.item.objectHandler.baseStats.description);
    }
}
