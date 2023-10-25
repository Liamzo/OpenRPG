using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFullAuto : BaseStrategy, ITrigger
{
    public float AttackHoldCost()
    {
        if (weapon.CanAttack()) {
            return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }

        return 0f;
    }

    public float AttackHold()
    {
        if (weapon.CanAttack()) {
            GameManager.instance.ShakeCamera(3.0f, 0.15f);

            weapon.CallOnTrigger();
            weapon.item.owner.GetComponent<Physicsable>().Knock(transform.up, weapon.statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());

            return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }
        return 0f;
    }

    public float AttackReleaseCost()
    {
         return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public float AttackRelease()
    {
        return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public void AttackCancel()
    {

    }
}
