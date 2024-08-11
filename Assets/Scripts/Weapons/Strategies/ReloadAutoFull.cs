using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Reload Auto Full", menuName = "Strategies/Reload Auto Full")]
public class ReloadAutoFull : BaseStrategy, IReload
{
    float reloadTimer = 0f;

    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnAttack += ResetReloadTimer;
    }

    public override void Delete()
    {
        weapon.triggerHolders[triggerSlot].OnAttack -= ResetReloadTimer;
    }

    public override void Update()
    {
        if (reloadTimer > 0f) {
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0f) {
                // Reload
                reloadTimer = 0f;
                weapon.CallOnReload((int)weapon.statsWeapon[WeaponStatNames.ClipSize].GetValue());
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
