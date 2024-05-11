using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackTypeComboSlash : BaseStrategy, IAttackType
{
    float lastCharge = 1f;
    bool inCombo = false;
    Vector3 lockedLookingDirection;


    [SerializeField] ComboSO combo;
    bool charging = false;

    float endHoldTimer = 0f;
    ComboAttack lastComboAttack;

    bool hasAvailableAttack = true;

    bool doingAttack = false;
    
    private void Start() {
        weapon.triggerHolders[triggerSlot].OnTrigger += ChargeAttack;
        weapon.triggerHolders[triggerSlot].OnTriggerRelease += DoAttack;

        weapon.item.OnUnequip += InteruptCombo;
    }

    private void LateUpdate() {
        if  (doingAttack && weapon.strategies.GetComponent<Collider2D>().enabled)
        {
            doingAttack = false;
            weapon.CallOnAttack(triggerSlot);
        }


        if (lastComboAttack != null && (weapon.animator.GetCurrentAnimatorStateInfo(0).IsName(lastComboAttack.attackAnimName) || weapon.animator.GetCurrentAnimatorStateInfo(0).IsName(lastComboAttack.attackHeavyAnimName)) && weapon.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f) {
            // Swing animation is complete, wait for the end hold duration before returning to Idle
            weapon.animator.speed = 1.0f;
            endHoldTimer += Time.deltaTime;

            if (lastComboAttack.comboChains.Count == 0 && hasAvailableAttack) {
                hasAvailableAttack = false;
                weapon._canAttack += 1;
            }

            if (endHoldTimer >= lastComboAttack.endHoldDuration) {
                ResetComboToIdle(); // Don't want to change the animation
            }
        } 
        else if (lastComboAttack != null && !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName(lastComboAttack.attackAnimName) && !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName(lastComboAttack.attackHeavyAnimName) && !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName(lastComboAttack.chargeAnimName)) 
        {
            // Started an attack but no longer playing one of the associated animations
            // Therefore, probably interupted by something, such as Blocking or getting hit
            Debug.Log("gooby");
            ResetCombo();
        }

        // TODO: Needs touched up, but not urgent
        if (inCombo) {
            // Limit looking rotation
            float angleToTarget = Vector3.Angle(lockedLookingDirection, weapon.item.owner.GetComponent<BaseBrain>().targetLookingDirection);
            float side = Mathf.Sign(Vector3.Cross(lockedLookingDirection, weapon.item.owner.GetComponent<BaseBrain>().targetLookingDirection).z); // Right is negative, Left is positive

            // Get new looking direction
            if (angleToTarget > 30f) {
                angleToTarget = 30f;
            }

            angleToTarget *= side;

            Vector3 newDir = Quaternion.AngleAxis(angleToTarget, Vector3.forward) * -lockedLookingDirection;

            // Set Looking Direction
            weapon.item.owner.GetComponent<BaseBrain>().SetLookingDirection(weapon.item.owner.GetComponent<EquipmentHandler>().orbitPoint.position + newDir);
        }
    }


    public void ChargeAttack(float charge) {
        if (charging == false) {
            if (!inCombo) {
                lastComboAttack = combo.comboChain;
                weapon.animator.Play(lastComboAttack.chargeAnimName);
                charging = true;
                return;
            }

            // Check which section we're in based on endHoldTimer/comboAttack.endHoldDuration, switch to that lastAttack
            float endHoldTimerPercent =  endHoldTimer / lastComboAttack.endHoldDuration;
            float segmentLength = lastComboAttack.endHoldDuration / lastComboAttack.comboChains.Count;

            int segment = Mathf.RoundToInt(endHoldTimerPercent / segmentLength);
            segment = Mathf.Clamp(segment, 0, lastComboAttack.comboChains.Count-1);

            if (lastComboAttack.comboChains.Count == 0) return;

            lastComboAttack = lastComboAttack.comboChains[segment];
            weapon.animator.Play(lastComboAttack.chargeAnimName);
            charging = true;
        }
    }

    public void DoAttack(float charge)
    {
        if (charging == false) return;
        charging = false;
        doingAttack = true;

        if (charge >= 100f && lastComboAttack.attackHeavyAnimName != "")
            weapon.animator.Play(lastComboAttack.attackHeavyAnimName);
        else
            weapon.animator.Play(lastComboAttack.attackAnimName);

        weapon.animator.speed = 1.0f / lastComboAttack.swingDuration;

        lastCharge = charge;

        lockedLookingDirection = weapon.item.owner.GetComponent<BaseBrain>().lookingDirection;

        if (!inCombo) {
            inCombo = true;

            // Slow down movement
            CharacterHandler character = (CharacterHandler) weapon.item.owner;
            character.statsCharacter[CharacterStatNames.MovementSpeed].AddModifier(-4f);
        }
    }

    void InteruptCombo() {
        if (inCombo) {
            ResetComboToIdle();
        }
    }

    void ResetComboToIdle() {
        weapon.animator.SetTrigger("Idle");
        weapon.animator.speed = 1.0f;

        ResetCombo();
    }
    void ResetCombo() {
        inCombo = false;
        lastComboAttack = null;

        CharacterHandler character = (CharacterHandler) weapon.item.owner;
        character.statsCharacter[CharacterStatNames.MovementSpeed].RemoveModifier(-4f);

        charging = false;

        endHoldTimer = 0f;

        if (hasAvailableAttack == false) {
            hasAvailableAttack = true;
            weapon._canAttack -= 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<ObjectHandler>(out ObjectHandler otherObjectHandler)) {
            if (otherObjectHandler == weapon.item.owner) return;
            
            weapon.triggerHolders[triggerSlot].damageType.DealDamage(otherObjectHandler, lastCharge);
        }

        if (other.TryGetComponent<BasicBullet>(out BasicBullet bullet)) {
            Destroy(bullet.gameObject);
        }
    }
}
