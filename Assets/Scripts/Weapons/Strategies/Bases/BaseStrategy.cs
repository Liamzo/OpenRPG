using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public abstract class BaseStrategy : MonoBehaviour
{
    protected WeaponHandler weapon;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        weapon = transform.parent.GetComponent<WeaponHandler>();
    }
}
