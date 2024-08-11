using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ammo Standard", menuName = "Strategies/Ammo Standard")]
public class AmmoStandard : BaseStrategy, IAmmo
{
    int currentAmmo;

    int _internalLock = 0;

    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        currentAmmo = (int)weapon.statsWeapon[WeaponStatNames.ClipSize].GetValue();

        weapon.OnReload += Reload;
        weapon.triggerHolders[triggerSlot].OnAttack += UseAmmo;

        LevelManager.instance.LoadLevelPre += LevelLoaded;
    }

    public override void Delete()
    {
        weapon.OnReload -= Reload;
        weapon.triggerHolders[triggerSlot].OnAttack -= UseAmmo;

        LevelManager.instance.LoadLevelPre -= LevelLoaded;

        if (_internalLock == 1) {
            weapon._canAttack -= 1;
            _internalLock -= 1;
        }
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return (int)weapon.statsWeapon[WeaponStatNames.ClipSize].GetValue();
    }

    public void Reload(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, (int)weapon.statsWeapon[WeaponStatNames.ClipSize].GetValue());

        if (currentAmmo >= weapon.GetStatValue(WeaponStatNames.AmmoCost)) {
            if (_internalLock == 1) {
                weapon._canAttack -= 1;
                _internalLock -= 1;
            }
        }
    }

    public void UseAmmo()
    {
        currentAmmo = Mathf.Clamp(currentAmmo - (int)weapon.GetStatValue(WeaponStatNames.AmmoCost), 0, (int)weapon.statsWeapon[WeaponStatNames.ClipSize].GetValue());

        if (currentAmmo < (int)weapon.GetStatValue(WeaponStatNames.AmmoCost)) {
            if (_internalLock == 0) {
                weapon._canAttack += 1;
                _internalLock += 1;
            }
        }
    }

    public void LevelLoaded() {
        currentAmmo = (int)weapon.statsWeapon[WeaponStatNames.ClipSize].GetValue();
    }
}
