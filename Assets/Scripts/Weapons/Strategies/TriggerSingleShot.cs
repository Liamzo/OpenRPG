using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSingleShot : BaseStrategy, ITrigger
{
    bool canFire = true;

    public float AttackHoldCost()
    {
        if (canFire && (rangedWeapon.ammo.GetCurrentAmmo() >= rangedWeapon.ammo.GetUseCost()) && rangedWeapon.fireRate.CanFire()) {
            return rangedWeapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }

        return 0f;
    }

    public float AttackHold()
    {
        if (canFire && (rangedWeapon.ammo.GetCurrentAmmo() >= rangedWeapon.ammo.GetUseCost()) && rangedWeapon.fireRate.CanFire()) {
            canFire = false;
            GameManager.instance.ShakeCamera(3.0f, 0.15f);

            rangedWeapon.attackType.DoAttack();
            rangedWeapon.ammo.UseAmmo();
            rangedWeapon.fireRate.FiredShot();
            rangedWeapon.item.owner.GetComponent<Physicsable>().Knock(transform.up, rangedWeapon.statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());

            return rangedWeapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }
        return 0f;
    }

    public float AttackReleaseCost()
    {
         return rangedWeapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public float AttackRelease()
    {
        canFire = true;
        return rangedWeapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public void AttackCancel()
    {
        canFire = true;
    }
}
