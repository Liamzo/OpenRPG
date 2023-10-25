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

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (transform.eulerAngles.z >= 5.0f && transform.eulerAngles.z <= 175.0f) {
            item.objectHandler.spriteRenderer.transform.localEulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
        } else if (transform.eulerAngles.z <= 355.0f && transform.eulerAngles.z >= 185.0f) {
            item.objectHandler.spriteRenderer.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
        }
    }


    public override void AttackAnticipation() {
    }
}
