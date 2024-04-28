using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSingleShot : BaseStrategy, ITrigger
{
    bool canAttack = true;

    public float AttackHoldCost()
    {
        return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
    }

    public bool CanAttackHold() {
        return canAttack && weapon.CanAttack();
    }

    public void AttackHold()
    {
        if (canAttack && weapon.CanAttack()) {
            canAttack = false;
            
            weapon.CallOnTrigger();
        }
    }

    public float AttackReleaseCost()
    {
         return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public bool CanAttackRelease() {
        return true;
    }

    public void AttackRelease()
    {
        canAttack = true;
    }

    public void AttackCancel()
    {
        canAttack = true;
    }
}
