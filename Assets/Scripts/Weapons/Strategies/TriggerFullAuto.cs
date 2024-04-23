using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFullAuto : BaseStrategy, ITrigger
{
    public float AttackHoldCost()
    {
        if (weapon.CanAttack()) {
            return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }

        return 0f;
    }

    public float AttackHold()
    {
        if (weapon.CanAttack()) {
            weapon.CallOnTrigger();

            return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }
        return 0f;
    }

    public float AttackReleaseCost()
    {
         return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public float AttackRelease()
    {
        weapon.CallOnTriggerRelease();
        
        return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public void AttackCancel()
    {

    }
}
