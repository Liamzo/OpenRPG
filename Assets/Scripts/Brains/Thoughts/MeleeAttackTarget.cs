using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackTarget : BaseThought
{
    bool attacking;
    float delayTimer = 0f;

    public float maxMeleeRange;

    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        float value = 0f;

        if (brain.threatHandler.target == null || !brain.character.objectStatusHandler.HasControls()) {
            return 0f;
        }

        WeaponHandler meleeWeapon = brain.equipmentHandler.currentEquipment[(int)EquipmentSlot.RightHand].GetComponent<WeaponHandler>();

        if (meleeWeapon == null) return 0f; // No melee weapon equipped

        if (brain.distToTarget > meleeWeapon.GetStatValue(WeaponStatNames.Range) && !brain.character.objectStatusHandler.HasMovementControls()) {
            return 0f;
        }

        if (brain.attackTimer <= 0 && brain.distToTarget <= maxMeleeRange) {
            value += 100f;
        }

        return value;
    }

    public override void Execute()
    {
        brain.equipmentHandler.rightMeleeSpot.weapon.Unholster(); // Temp, do better
        
        brain.SetTargetLookingDirection(brain.threatHandler.targetLastSeen.Value);

        if (brain.character.objectStatusHandler.HasMovementControls())
            brain.SetLookingDirection(brain.threatHandler.targetLastSeen.Value);
        
        brain.equipmentHandler.ToggleMeleeRanged(true);

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
                    weapon.AttackRelease(0);
                }

                brain.attackTimer = brain.attackCoolDown;
                delayTimer = 0f;
                attacking = false;
                brain.thoughtLocked = null;
            }
        } else {
            float dist = Vector2.Distance(brain.threatHandler.target.transform.position, transform.position);

            if (dist <= weapon.statsWeapon[WeaponStatNames.Range].GetValue()) {
                // Start the attack
                attacking = true;
                brain.thoughtLocked = this;
                weapon.AttackAnticipation(0);
            } else {
                // Chase target
                brain.movement += brain.GetDirectionFromPath() * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue();
            }
        }
    }
}
