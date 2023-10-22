using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoStandard : BaseStrategy, IAmmo
{
    int currentAmmo;
    public int useCost;

    void Start() {
        currentAmmo = (int)rangedWeapon.statsWeapon[WeaponStatNames.ClipSize].GetValue();
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return (int)rangedWeapon.statsWeapon[WeaponStatNames.ClipSize].GetValue();
    }

    public int GetUseCost()
    {
        return useCost;
    }

    public void Reload(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, (int)rangedWeapon.statsWeapon[WeaponStatNames.ClipSize].GetValue());
    }

    public void UseAmmo()
    {
        currentAmmo = Mathf.Clamp(currentAmmo - useCost, 0, (int)rangedWeapon.statsWeapon[WeaponStatNames.ClipSize].GetValue());
    }
}
