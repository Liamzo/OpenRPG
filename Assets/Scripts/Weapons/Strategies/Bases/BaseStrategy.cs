using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStrategy : MonoBehaviour
{
    protected RangedWeapon weapon;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        weapon = transform.parent.GetComponent<RangedWeapon>();
    }
}
