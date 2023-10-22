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
                meleeWeapon.AttackAnticipation();
                fullyCharged = true;
            }
        }
        
    }

    public float AttackHoldCost()
    {
        return meleeWeapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
    }

    public float AttackHold()
    {
        isCharging = true;
        meleeWeapon.animator.SetBool("Charging", true);
        return AttackHoldCost();
    }

    public float AttackReleaseCost()
    {
         return meleeWeapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public float AttackRelease()
    {
        meleeWeapon.animator.SetBool("Charging", false);
        isCharging = false;
        fullyCharged = false;
        chargeTimer = 0f;
        
        meleeWeapon.attackType.DoAttack();

        meleeWeapon.item.owner.GetComponent<Physicsable>().Knock(-transform.up, meleeWeapon.statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());

        return AttackReleaseCost();
    }

    public void AttackCancel()
    {
        meleeWeapon.animator.SetBool("Charging", false);
        isCharging = false;
        fullyCharged = false;
        chargeTimer = 0f;
    }
}
