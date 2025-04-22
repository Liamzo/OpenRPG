using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTarget : BaseThought
{
    bool attacking;
    float delayTimer = 0f;

    public float minAttackRange;

    public int maxAttacks;

    Coroutine doAttacks = null;
    
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
        if (attacking) return;

        brain.SetTargetLookingDirection(brain.threatHandler.TargetLastSeen.Value);

        if (brain.character.objectStatusHandler.HasMovementControls())
            brain.SetLookingDirection(brain.threatHandler.TargetLastSeen.Value);
        
        brain.equipmentHandler.ToggleMeleeRanged(false);

        WeaponHandler weapon = brain.equipmentHandler.rightMeleeSpot.weapon;

        attacking = true;
        brain.thoughtLocked = this;
        weapon.AttackAnticipation(0);
        StartCoroutine(DoAttacks());

        if (attacking) {
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
        }
    }

    public override void Cancel()
    {
        if (!attacking) return;

        if (doAttacks != null) StopCoroutine(doAttacks);
        doAttacks = null;

        attacking = false;
        brain.thoughtLocked = null;
        brain.ResetAttackCoolDown();
    }

    public IEnumerator DoAttacks() {
        WeaponHandler weapon = brain.equipmentHandler.rightMeleeSpot.weapon;        
        int attacks = Random.Range(0, maxAttacks);
        
        delayTimer = 0.2f;

        for (int i = 0; i <= attacks; i++) {
            while (delayTimer > 0f) {
                delayTimer -= Time.deltaTime;

                yield return null;
            }

            // Do the attack
            if (weapon != null) {
                brain.SetLookingDirection(brain.threatHandler.TargetLastSeen.Value);
                weapon.AttackHold(0);
                weapon.AttackRelease(0);
            }

            delayTimer = 0.2f;
        }

        Cancel();
    }
}
