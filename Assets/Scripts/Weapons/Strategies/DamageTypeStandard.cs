using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTypeStandard : BaseStrategy, IDamageType
{
    public void DealDamage(ObjectHandler target, float charge)
    {
        target.GetHit(weapon, (CharacterHandler) weapon.item.owner);
    }
}
