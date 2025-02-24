using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTarget : BaseThought
{
    bool attacking;
    float delayTimer = 0f;

    public float minAttackRange;
    
    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        float value = 0f;

        if (brain.threatHandler.Target == null || brain.threatHandler.LineOfSightToTarget.TargetInLineOfSight == false || !brain.character.objectStatusHandler.HasControls()) {
            return 0f;
        }

        if (brain.attackTimer <= 0f && brain.distToTarget >= minAttackRange) {
            value += 100f;
        }

        // Need to add check for In Range
        return value;
    }

    public override void Execute()
    {
        brain.SetTargetLookingDirection(brain.threatHandler.TargetLastSeen.Value);

        if (brain.character.objectStatusHandler.HasMovementControls())
            brain.SetLookingDirection(brain.threatHandler.TargetLastSeen.Value);
        
        brain.equipmentHandler.ToggleMeleeRanged(false);

        WeaponHandler weapon = brain.equipmentHandler.rightMeleeSpot.weapon;

        if (attacking) {
            if (!brain.character.objectStatusHandler.HasControls()) {
                // If we lose controls during anticipation, cancel the attack thought
                //brain.attackTimer = brain.attackCoolDown; // Could doing something with this
                delayTimer = 0f;
                attacking = false;
                brain.thoughtLocked = null;
                return;
            }
            
            delayTimer += Time.deltaTime;

            if (delayTimer >= 0.2f) {
                // Do the attack
                if (weapon != null) {
                    weapon.AttackHold(0);
                    weapon.AttackRelease(0);
                }

                brain.ResetAttackCoolDown();
                delayTimer = 0f;
                attacking = false;
                brain.thoughtLocked = null;
            }
        } else {
            attacking = true;
            brain.thoughtLocked = this;
            weapon.AttackAnticipation(0);
        }
    }
}
