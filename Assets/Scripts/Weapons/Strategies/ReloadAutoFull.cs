using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAutoFull : BaseStrategy, IReload
{
    float reloadTimer = 0f;

    protected override void Awake() {
        base.Awake();
        
        rangedWeapon.OnAttack += ResetReloadTimer;
    }

    public void Update()
    {
        if (reloadTimer > 0f) {
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0f) {
                // Reload
                reloadTimer = 0f;
                rangedWeapon.ammo.Reload((int)rangedWeapon.statsWeapon[WeaponStatNames.ClipSize].GetValue());
            }
        }
    }

    public void ReloadUpdate()
    {
        
    }

    public float ReloadPercentage() {
        if (reloadTimer <= 0f) {
            return 1f;
        }

        return (rangedWeapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue() - reloadTimer) / rangedWeapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue();
    }

    void ResetReloadTimer() {
        reloadTimer = rangedWeapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue();
    }
}
