using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackTarget : BaseThought
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

        if (brain.attackTimer <= 0) {
            value += 100f;
        }

        return value;
    }

    public override void Execute()
    {
        BaseWeaponHandler weapon = brain.GetComponent<EquipmentHandler>().GetEquipment(EquipmentSlot.RightHand).GetComponent<Weapon>();

        if (attacking) {
            delayTimer += Time.deltaTime;

            if (delayTimer >= 0.2f) {
                // Do the attack
                if (weapon != null) {
                    weapon.AttackEnd();
                }

                brain.attackTimer = brain.attackCoolDown;
                delayTimer = 0f;
                attacking = false;
                brain.thoughtLocked = null;
            }

            return;
        }

        float dist = Vector2.Distance(brain.threatHandler.target.transform.position, transform.position);

        if (dist <= weapon.statsWeapon[WeaponStatNames.Range].GetValue() * 0.8f) {
            // Start the attack
            attacking = true;
            brain.thoughtLocked = this;
            weapon.AttackAnticipation();
        } else {
            // Chase target
            brain.movement += brain.GetDirectionFromPath() * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue();
        }
    }
}