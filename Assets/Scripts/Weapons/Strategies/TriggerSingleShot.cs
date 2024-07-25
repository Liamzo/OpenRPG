using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Trigger SingleShot", menuName = "Strategies/Trigger SingleShot")]
public class TriggerSingleShot : BaseStrategy, ITrigger
{
    bool canAttack = true;


    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnAttackCancel += AttackCancel;
    }
    

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
        canAttack = true;

        weapon.CallOnTriggerRelease(triggerSlot);
    }

    public void AttackCancel()
    {
        canAttack = true;
    }
}
