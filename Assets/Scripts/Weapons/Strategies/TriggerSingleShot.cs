using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSingleShot : BaseStrategy, ITrigger
{
    bool canAttack = true;

    public float AttackHoldCost()
    {
        if (canAttack && weapon.CanAttack()) {
            return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }

        return 0f;
    }

    public float AttackHold()
    {
        if (canAttack && weapon.CanAttack()) {
            canAttack = false;
            
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
        canAttack = true;
        return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public void AttackCancel()
    {
        canAttack = true;
    }
}
