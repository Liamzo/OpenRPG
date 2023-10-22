using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateStandard : BaseStrategy, IFireRate
{
    float fireTimer = 0f;

    private void Update() {
        if (fireTimer > 0f) {
            fireTimer -= Time.deltaTime;
        }
    }

    public bool CanFire()
    {
        if (fireTimer <= 0f) {
            return true;
        }

        return false;
    }

    public void FiredShot()
    {
        fireTimer = rangedWeapon.statsWeapon[WeaponStatNames.AttackTimer].GetValue();
    }
}
