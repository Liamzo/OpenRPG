using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeProjectile : BaseStrategy, IAttackType
{
    public MuzzleFlash muzzleFlash;
    public GameObject bulletPrefab;
    public float bulletSpeed;

    public void DoAttack()
    {
        GameObject go = Instantiate(bulletPrefab, rangedWeapon.attackPoint.position, Quaternion.identity);
        LizardBullet bullet = go.GetComponent<LizardBullet>();
        bullet.weapon = rangedWeapon;
        bullet.bulletSpeed = bulletSpeed;
        bullet.direction = -transform.up;
        muzzleFlash.Flash();
        rangedWeapon.CallOnAttack();
    }
}
