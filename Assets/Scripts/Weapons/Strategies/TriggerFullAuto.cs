using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFullAuto : BaseStrategy, ITrigger
{
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnAttackCancel += AttackCancel;
    }

    
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
            weapon.CallOnTrigger(triggerSlot);
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
        weapon.CallOnTriggerRelease(triggerSlot);
    }

    public void AttackCancel()
    {

    }
}
