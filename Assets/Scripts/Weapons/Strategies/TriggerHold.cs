using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHold : BaseStrategy, ITrigger
{
    private void Start() {
        weapon.triggerHolders[triggerSlot].OnAttackCancel += AttackCancel;
    }

    
    public float AttackHoldCost()
    {
        return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
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
         return 0f;
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
