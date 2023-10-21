using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFullAuto : BaseStrategy, ITrigger
{
    public float AttackHoldCost()
    {
        if ((weapon.ammo.GetCurrentAmmo() >= weapon.ammo.GetUseCost()) && weapon.fireRate.CanFire()) {
            return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }

        return 0f;
    }

    public float AttackHold()
    {
        if ((weapon.ammo.GetCurrentAmmo() >= weapon.ammo.GetUseCost()) && weapon.fireRate.CanFire()) {
            GameManager.instance.ShakeCamera(3.0f, 0.15f);

            weapon.attackType.DoAttack();
            weapon.ammo.UseAmmo();
            weapon.fireRate.FiredShot();
            weapon.CallOnAttack();
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
