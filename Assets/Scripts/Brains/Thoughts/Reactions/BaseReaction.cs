using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseReaction : BaseThought
{
    public abstract float Evaluate(WeaponHandler weapon, GameObject projectile);


    // Probably a better way to do this
    public override float Evaluate() {
        return 0f;
    }
}
