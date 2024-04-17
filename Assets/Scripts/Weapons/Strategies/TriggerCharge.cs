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
        if (weapon.CanAttack()) {
            return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
        }

        return 0f;
    }

    public float AttackHold()
    {
        if (weapon.CanAttack()) {
            isCharging = true;
            weapon.animator.SetBool("Charging", true);

            weapon.CallOnTrigger(chargeTimer);

            return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
        } else {
            AttackCancel();
            return 0f;
        }
    }

    public float AttackReleaseCost()
    {
         return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public float AttackRelease()
    {
        if (weapon.CanAttack()) {
            weapon.CallOnTriggerRelease(chargeTimer);

            AttackCancel();
            
            weapon.item.owner.GetComponent<Physicsable>().Knock(-transform.up, weapon.statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());

            return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
        } else {
            AttackCancel();
            return 0f;
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
