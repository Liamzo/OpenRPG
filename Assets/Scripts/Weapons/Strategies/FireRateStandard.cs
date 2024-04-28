using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateStandard : BaseStrategy, IFireRate
{
    float fireTimer = 0f;
    bool waiting = false;

    void Start() {
        weapon.triggerHolders[triggerSlot].OnAttack += DidAttack;
    }

    private void Update() {
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
        fireTimer = weapon.statsWeapon[WeaponStatNames.AttackTimer].GetValue();
        weapon._canAttack += 1;
        waiting = true;
    }
}
