using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTarget : BaseThought
{
    bool attacking;
    float delayTimer = 0f;
    
    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        float value = 0f;

        if (brain.threatHandler.target == null) {
            return 0f;
        }

        if (brain.attackTimer <= 0f) {
            value += 100f;
        }

        // Need to add check for In Range. Probably in FindSightLine too

        return value;
    }

    public override void Execute()
    {
        brain.equipmentHandler.ToggleMeleeRanged(false);

        WeaponHandler weapon = brain.equipmentHandler.rightMeleeSpot.weapon;

        if (attacking) {
            delayTimer += Time.deltaTime;

            if (delayTimer >= 0.2f) {
                // Do the attack
                if (weapon != null) {
                    weapon.AttackHold();
                    weapon.AttackRelease();
                }

                brain.attackTimer = brain.attackCoolDown;
                delayTimer = 0f;
                attacking = false;
                brain.thoughtLocked = null;
            }
        } else {
            attacking = true;
            brain.thoughtLocked = this;
            weapon.AttackAnticipation();
        }
    }
}
