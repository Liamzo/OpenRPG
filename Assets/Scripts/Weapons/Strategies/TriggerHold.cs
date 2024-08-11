using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Trigger Hold", menuName = "Strategies/Trigger Hold")]
public class TriggerHold : BaseStrategy, ITrigger
{
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnAttackCancel += AttackCancel;
    }

    public override void Delete()
    {
        weapon.triggerHolders[triggerSlot].OnAttackCancel -= AttackCancel;
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
