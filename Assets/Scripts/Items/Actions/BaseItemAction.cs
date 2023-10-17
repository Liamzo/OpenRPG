using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemAction : ScriptableObject
{
    public string menuName;
    
    public abstract void Action(ItemAction action);

    public virtual string MakeMenuName(ItemAction action) {
        return menuName;
    }
}
