using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Trigger Charge", menuName = "Strategies/Trigger Charge")]
public class TriggerCharge : BaseStrategy, ITrigger
{
    bool isCharging = false;
    bool fullyCharged = false;
    float chargeTimer = 0f;


    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnAttackCancel += AttackCancel;
    }

    public override void Update()
    {
        if (isCharging && !fullyCharged) {
            chargeTimer += Time.deltaTime;

            if (chargeTimer > 1f) {
                chargeTimer = 1f;
                weapon.triggerHolders[triggerSlot].AttackAnticipation();
                fullyCharged = true;
            }
        }  
    }

    public float AttackHoldCost()
    {
        return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
    }

    public bool CanAttackHold() {
        bool canAttackHold = weapon.CanAttack();

        if (!canAttackHold)
            AttackCancel();

        return canAttackHold;
    }

    public void AttackHold()
    {
        if (weapon.CanAttack()) {
            isCharging = true;
            weapon.animator.SetBool("Charging", true);

            weapon.CallOnTrigger(triggerSlot, chargeTimer);
        } else {
            AttackCancel();
        }
    }

    public float AttackReleaseCost()
    {
         return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public bool CanAttackRelease() {
        return weapon.CanAttack();
    }

    public void AttackRelease()
    {
        if (weapon.CanAttack()) {
            weapon.CallOnTriggerRelease(triggerSlot, chargeTimer);

            AttackCancel();
        } else {
            AttackCancel();
        }
    }

    public void AttackCancel()
    {
        weapon.animator.SetBool("Charging", false);
        isCharging = false;
        fullyCharged = false;
        chargeTimer = 0f;
    }
}
