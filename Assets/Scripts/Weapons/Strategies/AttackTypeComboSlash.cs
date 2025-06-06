using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combo Slash", menuName = "Strategies/Combo Slash")]
public class AttackTypeComboSlash : BaseStrategy, ITrigger, IAttackType
{
    float lastCharge = 1f;
    bool inCombo = false;
    Vector3 lockedLookingDirection;


    [SerializeField] GameObject trailPrefab;
    TrailRenderer trailRenderer;
    [SerializeField] ComboSO combo;
    bool charging = false;

    float endHoldTimer = 0f;
    ComboAttack currentComboAttack;
    AttackDetails currentAttack;
    ComboAttack prevComboAttack;
    AttackDetails prevAttack;

    bool hasAvailableAttack = true;

    bool doingAttack = false;
    bool endingAttack = false;




    bool isCharging = false;
    bool fullyCharged = false;
    float chargeTimer = 0f;
    
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);

        trailRenderer = Instantiate(trailPrefab, weapon.item.objectHandler.spriteRenderer.transform).GetComponent<TrailRenderer>();
        
        weapon.item.OnUnequip += InteruptCombo;

        weapon.triggerHolders[triggerSlot].OnAttackCancel += AttackCancel;

        weapon.meleeHitbox.GetComponent<WeaponHitDetector>().TriggerEntered += OnTriggerEnter2D;
    }

    public override void Delete()
    {
        weapon.item.OnUnequip -= InteruptCombo;

        weapon.triggerHolders[triggerSlot].OnAttackCancel -= AttackCancel;

        weapon.meleeHitbox.GetComponent<WeaponHitDetector>().TriggerEntered -= OnTriggerEnter2D;
    }

    public override void Update()
    {
        if (isCharging && !fullyCharged) {
            chargeTimer += CalculateAnimationChargeSpeed() * Time.deltaTime;

            if (chargeTimer > 1f) {
                chargeTimer = 1f;
                weapon.triggerHolders[triggerSlot].AttackAnticipation();
                fullyCharged = true;
            }
        }  
    }

    public override void LateUpdate() {
        if  (doingAttack && weapon.meleeHitbox.enabled)
        {
            doingAttack = false;
            endingAttack = true;
            weapon.CallOnAttack(triggerSlot);
        }


        if (currentComboAttack != null && currentAttack != null && weapon.animator.GetCurrentAnimatorStateInfo(0).IsName(currentAttack.attackAnimName) && weapon.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f) {
            // Swing animation is complete, wait for the end hold duration before returning to Idle
            if (endingAttack) {
                weapon.animator.speed = 1.0f;
                endingAttack = false;
                trailRenderer.enabled = false;

                weapon.statsWeapon[WeaponStatNames.KnockBack].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.knockBackModifier));
                weapon.statsWeapon[WeaponStatNames.SelfKnockForce].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.selfKnockBackModifier));
                weapon.statsWeapon[WeaponStatNames.Stagger].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.staggerModifier));
            }

            endHoldTimer += Time.deltaTime;

            if (currentComboAttack.comboChains.Count == 0 && hasAvailableAttack) {
                hasAvailableAttack = false;
                weapon._canAttack += 1;
            }

            if (endHoldTimer >= currentComboAttack.endHoldDuration) {
                ResetComboToIdle(); // Don't want to change the animation
            }
        } 
        else if (currentComboAttack != null && currentAttack != null && !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName(currentAttack.attackAnimName) && !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName(currentComboAttack.chargeAnimName)) 
        {
            // Started an attack but no longer playing one of the associated animations
            // Therefore, probably interupted by something, such as Blocking or getting hit
            ResetCombo();
        }

        // TODO: Change to Lower speed of Rotation rather than locking. Attacks are already locked in anyway
        // if (inCombo) {
        //     // Limit looking rotation
        //     float angleToTarget = Vector3.Angle(lockedLookingDirection, weapon.item.owner.GetComponent<BaseBrain>().targetLookingDirection);
        //     float side = Mathf.Sign(Vector3.Cross(lockedLookingDirection, weapon.item.owner.GetComponent<BaseBrain>().targetLookingDirection).z); // Right is negative, Left is positive

        //     // Get new looking direction
        //     if (angleToTarget > 30f) {
        //         angleToTarget = 30f;
        //     }

        //     angleToTarget *= side;

        //     Vector3 newDir = Quaternion.AngleAxis(angleToTarget, Vector3.forward) * lockedLookingDirection;

        //     // Set Looking Direction
        //     weapon.item.owner.GetComponent<BaseBrain>().SetLookingDirection(weapon.item.owner.GetComponent<EquipmentHandler>().orbitPoint.position + newDir);
        // }
    }

    public void DoAttack(float charge)
    {
        if (charging == false) return;
        charging = false;
        doingAttack = true;

        endHoldTimer = 0f;

        if (charge >= 1f && currentComboAttack.heavyAttack.attackAnimName != ""){
            currentAttack = currentComboAttack.heavyAttack;
        } 
        else if(charge >= 1f && currentComboAttack.heavyAttack.attackAnimName == "")
        {
            currentAttack = currentComboAttack.heavyAttack;
            currentAttack.attackAnimName = currentComboAttack.lightAttack.attackAnimName;
        }
        else {
            currentAttack = currentComboAttack.lightAttack;
        }

        weapon.animator.Play(currentAttack.attackAnimName);
        weapon.animator.speed = CalculateAnimationSwingSpeed();
        trailRenderer.enabled = true;

        weapon.statsWeapon[WeaponStatNames.KnockBack].AddModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.knockBackModifier));
        weapon.statsWeapon[WeaponStatNames.SelfKnockForce].AddModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.selfKnockBackModifier));
        weapon.statsWeapon[WeaponStatNames.Stagger].AddModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.staggerModifier));

        lastCharge = charge;

        lockedLookingDirection = weapon.item.owner.GetComponent<BaseBrain>().lookingDirection;

        if (!inCombo) {
            inCombo = true;

            // Slow down movement
            CharacterHandler character = (CharacterHandler) weapon.item.owner;
            character.statsCharacter[CharacterStatNames.MovementSpeed].AddModifier(new Modifier(ModifierTypes.Multiplier, -90f));
        }

        weapon.item.owner.GetComponent<ThreatHandler>().CallOnStartAttack(weapon, null);
    }

    void InteruptCombo() {
        if (inCombo) {
            ResetComboToIdle();
        }
    }

    void ResetComboToIdle() {
        weapon.animator.SetTrigger("Idle");
        weapon.animator.speed = 1.0f;
        trailRenderer.enabled = false;

        ResetCombo();
    }
    void ResetCombo() {
        inCombo = false;
        prevComboAttack = currentComboAttack;
        prevAttack = currentAttack;
        currentComboAttack = null;
        currentAttack = null;

        CharacterHandler character = (CharacterHandler) weapon.item.owner;
        character.statsCharacter[CharacterStatNames.MovementSpeed].RemoveModifier(new Modifier(ModifierTypes.Multiplier, -90f));

        charging = false;

        endHoldTimer = 0f;

        if (hasAvailableAttack == false) {
            hasAvailableAttack = true;
            weapon._canAttack -= 1;
        }
    }


    public float AttackHoldCost()
    {
        return weapon.statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
    }

    public bool CanAttackHold() {
        bool canAttackHold = weapon.CanAttack();

        if (!canAttackHold)
            AttackCancel();

        return canAttackHold;
    }

    public void AttackHold()
    {
        if (weapon.CanAttack()) {
            isCharging = true;

            if (charging == false) {
                if (!inCombo) {
                    prevComboAttack = currentComboAttack;
                    prevAttack = currentAttack;
                    currentComboAttack = combo.comboChain;
                    currentAttack = null;
                    weapon.animator.Play(currentComboAttack.chargeAnimName);
                    weapon.animator.speed = CalculateAnimationChargeSpeed();
                    trailRenderer.enabled = false;
                    charging = true;
                    return;
                }

                if (currentComboAttack.comboChains.Count == 0) return;

                
                // Check which section we're in based on endHoldTimer/comboAttack.endHoldDuration, switch to that lastAttack
                float endHoldTimerPercent =  endHoldTimer / currentComboAttack.endHoldDuration;
                float segmentLength = currentComboAttack.endHoldDuration / currentComboAttack.comboChains.Count;

                int segment = Mathf.RoundToInt(endHoldTimerPercent / segmentLength);
                segment = Mathf.Clamp(segment, 0, currentComboAttack.comboChains.Count-1);


                prevComboAttack = currentComboAttack;
                prevAttack = currentAttack;
                currentComboAttack = currentComboAttack.comboChains[segment];
                currentAttack = null;
                weapon.animator.Play(currentComboAttack.chargeAnimName);
                weapon.animator.speed = CalculateAnimationChargeSpeed();
                trailRenderer.enabled = false;
                charging = true;
            }

            weapon.CallOnTrigger(triggerSlot, chargeTimer);
        } else {
            AttackCancel();
        }
    }

    public float AttackReleaseCost()
    {
         return weapon.statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public bool CanAttackRelease() {
        return weapon.CanAttack();
    }

    public void AttackRelease()
    {
        if (weapon.CanAttack()) {
            DoAttack(chargeTimer);

            weapon.CallOnTriggerRelease(triggerSlot, chargeTimer);

            isCharging = false;
            fullyCharged = false;
            chargeTimer = 0f;
        } else {
            AttackCancel();
        }
    }

    public void AttackCancel()
    {
        isCharging = false;
        fullyCharged = false;
        chargeTimer = 0f;
        trailRenderer.enabled = false;

        if (charging) {
            currentComboAttack = prevComboAttack;
            currentAttack = prevAttack;
            charging = false;

            if (currentComboAttack == null) {
                weapon.animator.SetTrigger("Idle");
                weapon.animator.speed = 1.0f;
            } else if (prevAttack != null) {
                weapon.animator.Play(currentComboAttack.lightAttack.attackAnimName, 0, 1.1f); // TODO: Save if we did a normal or heavy attack
                weapon.animator.speed = CalculateAnimationSwingSpeed();
            }
        }

        if (doingAttack) {
            // TODO: Don't know if this works
            weapon.statsWeapon[WeaponStatNames.KnockBack].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.knockBackModifier));
            weapon.statsWeapon[WeaponStatNames.SelfKnockForce].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.selfKnockBackModifier));
            weapon.statsWeapon[WeaponStatNames.Stagger].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.staggerModifier));

            weapon.animationUnblockAttack();

            currentComboAttack = prevComboAttack;
            currentAttack = prevAttack;
            doingAttack = false;

            if (currentComboAttack == null) {
                ResetComboToIdle();
            } else if (prevAttack != null) {
                // TODO: Don't know if this works, need to implement either enemy combos or enemy blocking first
                weapon.animator.Play(currentComboAttack.lightAttack.attackAnimName, 0, 1.1f); // TODO: Save if we did a normal or heavy attack
                weapon.animator.speed = CalculateAnimationSwingSpeed();
            }
        }
        
        if (endingAttack) {
            weapon.statsWeapon[WeaponStatNames.KnockBack].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.knockBackModifier));
            weapon.statsWeapon[WeaponStatNames.SelfKnockForce].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.selfKnockBackModifier));
            weapon.statsWeapon[WeaponStatNames.Stagger].RemoveModifier(new Modifier(ModifierTypes.Multiplier, currentAttack.staggerModifier));

            weapon.animationUnblockAttack();

            currentComboAttack = prevComboAttack;
            currentAttack = prevAttack;
            endingAttack = false;

            if (currentComboAttack == null) {
                ResetComboToIdle();
            } else if (prevAttack != null) {
                // TODO: Don't know if this works, need to implement either enemy combos or enemy blocking first
                weapon.animator.Play(currentComboAttack.lightAttack.attackAnimName, 0, 1.1f); // TODO: Save if we did a normal or heavy attack
                weapon.animator.speed = CalculateAnimationSwingSpeed();
            }
        }
    }

    float CalculateAnimationSwingSpeed() {
        return weapon.GetStatValue(WeaponStatNames.AttackSpeed) * currentAttack.swingModifier; // Higher is faster
    }
    float CalculateAnimationChargeSpeed() {
        return (1.0f / currentComboAttack.chargeDuration) * (weapon.GetStatValue(WeaponStatNames.AttackSpeed) * 0.5f);
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.TryGetComponent<ObjectHandler>(out ObjectHandler otherObjectHandler) || (other.gameObject.transform.parent != null && other.gameObject.transform.parent.TryGetComponent<ObjectHandler>(out otherObjectHandler))) {
            if (otherObjectHandler == weapon.item.owner) return;
            
            weapon.CallOnHitTarget(triggerSlot, otherObjectHandler, lastCharge);
        } else {
            weapon.CallOnHitTarget(triggerSlot, null, lastCharge);
        }

        if (other.TryGetComponent<BasicBullet>(out BasicBullet bullet)) {
            Destroy(bullet.gameObject);
        }
    }
}
