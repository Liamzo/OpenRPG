using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : BaseWeaponHandler
{
    public IAmmo ammo;
    public IReload reload;
    public IFireRate fireRate;


    protected override void Awake()
    {
        base.Awake();

        // Handle cases where no Strategy is assigned
        ammo = strategies.GetComponent<IAmmo>();
        reload = strategies.GetComponent<IReload>();
        fireRate = strategies.GetComponent<IFireRate>();
    }
}
