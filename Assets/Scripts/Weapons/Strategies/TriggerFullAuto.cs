using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFullAuto : BaseStrategy, ITrigger
{
    public float AttackHoldCost()
    {
        return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
    }

    public bool CanAttackHold() {
        return weapon.CanAttack();
    }

    public void AttackHold()
    {
        if (weapon.CanAttack()) {
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
        weapon.CallOnTriggerRelease();
    }

    public void AttackCancel()
    {

    }
}
