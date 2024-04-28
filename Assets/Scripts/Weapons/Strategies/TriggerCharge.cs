using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCharge : BaseStrategy, ITrigger
{
    bool isCharging = false;
    bool fullyCharged = false;
    float chargeTimer = 0f;

    void Update()
    {
        if (isCharging && !fullyCharged) {
            chargeTimer += Time.deltaTime;

            if (chargeTimer > 1f) {
                chargeTimer = 1f;
                weapon.AttackAnticipation();
                fullyCharged = true;
            }
        }
        
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
            isCharging = true;
            weapon.animator.SetBool("Charging", true);

            weapon.CallOnPrimaryTrigger(chargeTimer);
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
            weapon.CallOnPrimaryTriggerRelease(chargeTimer);

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
