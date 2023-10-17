using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : BaseWeaponHandler
{
    bool canFire = true;
    public Transform bulletSpawn;
    public MuzzleFlash muzzleFlash;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public int currentAmmo;
    public float reloadTimer;

    protected override void Awake()
    {
        base.Awake();

        currentAmmo = (int)statsWeapon[WeaponStatNames.ClipSize].GetValue();
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

        if (reloadTimer > 0f) {
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0f) {
                // Reload
                reloadTimer = 0f;
                currentAmmo = (int)statsWeapon[WeaponStatNames.ClipSize].GetValue();
            }
        }
    }

    public override float AttackHoldCost()
    {
        if (canFire && currentAmmo > 0) {
            return statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }

        return 0f;
    }
    public override float AttackHold() {
        if (canFire && currentAmmo > 0) {
            //animator.SetTrigger("Attack");
            GameObject go = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            LizardBullet bullet = go.GetComponent<LizardBullet>();
            bullet.weapon = this;
            bullet.bulletSpeed = bulletSpeed;
            bullet.direction = -transform.up;
            canFire = false;
            GameManager.instance.ShakeCamera(3.0f, 0.15f);
            currentAmmo -= 1;
            reloadTimer = statsWeapon[WeaponStatNames.ReloadDuration].GetValue();
            muzzleFlash.Flash();

            item.owner.GetComponent<Physicsable>().Knock(transform.up, statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());

            return statsWeapon[WeaponStatNames.StaminaCostHold].GetValue();
        }
        return 0f;
    }

    public override float AttackEndCost()
    {
        return statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }
    public override float AttackEnd() {
        canFire = true;
        return statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public override void AttackCancel() {
        canFire = true;
    }


    public override void AttackAnticipation() {
    }
}
