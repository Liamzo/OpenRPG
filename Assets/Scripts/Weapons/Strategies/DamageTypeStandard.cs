using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTypeStandard : BaseStrategy, IDamageType
{
    public void DealDamage(ObjectHandler target)
    {
        target.GetHit(meleeWeapon, (CharacterHandler) meleeWeapon.item.owner);
    }
}
