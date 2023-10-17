using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteraction : MonoBehaviour
{
    public string menuName;
    
    public abstract void Interact(CharacterHandler character);

    public virtual string MakeMenuName(ItemAction action) {
        return menuName;
    }
}
