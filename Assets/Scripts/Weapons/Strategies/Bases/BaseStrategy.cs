using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public abstract class BaseStrategy : MonoBehaviour
{
    protected RangedWeapon rangedWeapon;
    protected MeleeWeapon meleeWeapon;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        transform.parent.TryGetComponent<RangedWeapon>(out rangedWeapon);
        transform.parent.TryGetComponent<MeleeWeapon>(out meleeWeapon);
    }
}
