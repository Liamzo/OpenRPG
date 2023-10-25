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
                weapon.AttackAnticipation();
                fullyCharged = true;
            }
        }
        
    }

    public float AttackHoldCost()
    {
        return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
    }

    public float AttackHold()
    {
        isCharging = true;
        weapon.animator.SetBool("Charging", true);
        return AttackHoldCost();
    }

    public float AttackReleaseCost()
    {
         return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public float AttackRelease()
    {
        weapon.animator.SetBool("Charging", false);
        isCharging = false;
        fullyCharged = false;
        chargeTimer = 0f;
        
        weapon.CallOnTrigger();

        weapon.item.owner.GetComponent<Physicsable>().Knock(-transform.up, weapon.statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());

        return AttackReleaseCost();
    }

    public void AttackCancel()
    {
        weapon.animator.SetBool("Charging", false);
        isCharging = false;
        fullyCharged = false;
        chargeTimer = 0f;
    }
}
