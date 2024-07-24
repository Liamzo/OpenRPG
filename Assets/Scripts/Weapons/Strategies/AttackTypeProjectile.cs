using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeProjectile : BaseStrategy, IAttackType
{
    [SerializeField] private GameObject muzzleFlashPrefab;
    MuzzleFlash muzzleFlash;

    public GameObject bulletPrefab;
    public float bulletSpeed;

    public override void Create() {
        base.Create();
        
        muzzleFlash = Instantiate(muzzleFlashPrefab, weapon.attackPoint.transform).GetComponent<MuzzleFlash>();

        weapon.triggerHolders[triggerSlot].OnTrigger += DoAttack;
    }

    public void DoAttack(float charge)
    {
        GameObject go = Instantiate(bulletPrefab, weapon.attackPoint.position, Quaternion.identity);
        BasicBullet bullet = go.GetComponent<BasicBullet>();
        bullet.weapon = weapon;
        bullet.bulletSpeed = bulletSpeed;
        bullet.weaponCharge = charge;
        bullet.direction = -transform.up;
        muzzleFlash.Flash();
        weapon.CallOnAttack(triggerSlot);
    }
}
