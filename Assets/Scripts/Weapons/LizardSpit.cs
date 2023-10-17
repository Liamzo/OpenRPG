using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardSpit : BaseWeaponHandler
{
    float attackTimer = 0f;

    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public float bulletSpeed;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        if (attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }
    }

    public override float AttackHoldCost()
    {
        return 0f;
    }
    public override float AttackHold() {
        if (attackTimer <= 0) {
            //animator.SetTrigger("Attack");
            GameObject go = Instantiate(bulletPrefab, bulletSpawn);
            LizardBullet bullet = go.GetComponent<LizardBullet>();
            bullet.weapon = this;
            bullet.bulletSpeed = bulletSpeed;
            bullet.direction = (transform.forward - bulletSpawn.position).normalized;
            attackTimer = statsWeapon[WeaponStatNames.AttackTimer].GetValue();
        }
        return 0f;
    }

    public override float AttackEndCost()
    {
        return 0f;
    }
    public override float AttackEnd() {
        return 0f;
    }

    public override void AttackCancel() {
    }


    public override void AttackAnticipation() {
    }
}
