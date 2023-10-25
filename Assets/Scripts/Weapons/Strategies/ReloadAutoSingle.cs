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

    public void Update()
    {
        if (reloadTimer > 0f) {
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0f) {
                // Reload
                reloadTimer = 0f;
                weapon.CallOnReload(1);

                if (weapon.CanReload()) {
                    ResetReloadTimer();
                }
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

        return (weapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue() - reloadTimer) / weapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue();
    }

    void ResetReloadTimer() {
        reloadTimer = weapon.statsWeapon[WeaponStatNames.ReloadDuration].GetValue();
    }
}
