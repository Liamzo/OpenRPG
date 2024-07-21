using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAction
{
    public ItemHandler item;
    public BaseItemAction action;
    public Dictionary<ItemActionDataType, object> data;

    public string menuName;

    public ItemAction (ItemHandler item, BaseItemAction action, Dictionary<ItemActionDataType, object> data = null) {
        this.item = item;
        this.action = action;
        if (data == null)
            this.data = new Dictionary<ItemActionDataType, object>();
        else {
            this.data = data;
            foreach(ItemActionDataType key in data.Keys) {
                this.menuName += " " + data[key].ToString();
            }
        }
        this.menuName = action.MakeMenuName(this);
    }

    public void Action() {
        action.Action(this);
    }

    public bool CanPerform() {
        return action.CanPerform(this);
    }
}

public enum ItemActionDataType {
    EquipSlot
}
