using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAutoSingle : BaseStrategy, IReload
{
    float reloadTimer = 0f;

    protected override void Awake() {
        base.Awake();
        
        weapon.OnAttack += ResetReloadTimer;
    }

    public void ReloadUpdate()
    {
        if (reloadTimer > 0f) {
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0f) {
                // Reload
                reloadTimer = 0f;
                weapon.ammo.Reload(1);

                if (weapon.ammo.GetCurrentAmmo() < weapon.ammo.GetMaxAmmo()) {
                    ResetReloadTimer();
                }
            }
        }
    }

    public float ReloadPercentage() {
        if (reloadTimer <= 0f) {
            return 1f;
        }

        return (weapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue() - reloadTimer) / weapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue();
    }

    void ResetReloadTimer() {
        reloadTimer = weapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue();
    }
}
