using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackTarget : BaseThought
{
    bool attacking;
    float delayTimer = 0f;

    public float maxMeleeRange; // Currently used by Ranged Bandit to determine when to melee and ranged, could probably do this a better way during Evaluation. If lower than something like the Max Circle Distance, Melee Bandits will sometimes never attack
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
        if (attacking) return;

        brain.equipmentHandler.rightMeleeSpot.weapon.Unholster(); // Temp, do better
        
        brain.SetTargetLookingDirection(brain.threatHandler.TargetLastSeen.Value);

        if (brain.character.objectStatusHandler.HasMovementControls())
            brain.SetLookingDirection(brain.threatHandler.TargetLastSeen.Value);
        
        brain.equipmentHandler.ToggleMeleeRanged(true);

        WeaponHandler weapon = brain.equipmentHandler.rightMeleeSpot.weapon;

        float dist = Vector2.Distance(brain.threatHandler.Target.transform.position, transform.position);

        if (dist <= weapon.statsWeapon[WeaponStatNames.Range].GetValue()) {
            // Start the attack
            attacking = true;
            brain.thoughtLocked = this;
            weapon.AttackAnticipation(0);
            doAttacks = StartCoroutine(DoAttacks());
        } else {
            // Chase target
            brain.movement += brain.FindPossibleDirectionFromIdeal(brain.GetDirectionFromPath()) * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 2f;
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
        delayTimer = 0.2f;

        int attacks = Random.Range(0, maxAttacks);

        for (int i = 0; i <= attacks; i++) {
            while (delayTimer > 0f) {
                delayTimer -= Time.deltaTime;

                yield return null;
            }

            WeaponHandler weapon = brain.equipmentHandler.rightMeleeSpot.weapon;        

            // Do the attack
            if (weapon != null) {
                weapon.AttackHold(0);
                weapon.AttackRelease(0);
            }

            delayTimer = 0.5f;
        }

        Cancel();
    }
}
