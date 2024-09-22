using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FireRate Standard", menuName = "Strategies/FireRate Standard")]
public class FireRateStandard : BaseStrategy, IFireRate
{
    float fireTimer = 0f;
    bool waiting = false;

    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnAttack += DidAttack;
    }

    public override void Delete()
    {
        weapon.triggerHolders[triggerSlot].OnAttack -= DidAttack;
    }

    public override void Update() {
        if (fireTimer > 0f) {
            fireTimer -= Time.deltaTime;
        }

        if (fireTimer <= 0f && waiting) {
            weapon._canAttack -= 1;
            waiting = false;
        }
    }

    public bool CanFire()
    {
        if (fireTimer <= 0f) {
            return true;
        }

        return false;
    }

    public void DidAttack()
    {
        fireTimer = 1.0f / weapon.statsWeapon[WeaponStatNames.AttackSpeed].GetValue(); // Lower is faster
        weapon._canAttack += 1;
        waiting = true;
    }
}
