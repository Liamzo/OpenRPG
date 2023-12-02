using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackTypeComboSlash : BaseStrategy, IAttackType
{
    float lastCharge = 1f;
    bool inCombo = false;
    Vector3 lockedLookingDirection;
    
    private void Start() {
        weapon.OnTrigger += DoAttack;
        weapon.OnAttack += SpawnSlashAnimEvent;

        weapon.item.OnUnequip += InteruptCombo;
    }

    private void Update() {
        if (weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Idle") && inCombo) {
            inCombo = false;

            weapon.item.owner.objectStatusHandler.UnblockMovementControls();
        } else if (inCombo) {
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

    public void DoAttack(float charge)
    {
        weapon.animator.SetTrigger("Attack");
        lastCharge = charge;

        lockedLookingDirection = weapon.item.owner.GetComponent<BaseBrain>().lookingDirection;

        if (!inCombo) {
            inCombo = true;

            // Disable movement
            weapon.item.owner.objectStatusHandler.BlockMovementControls();
        }
    }

    public void SpawnSlashAnimEvent() {
        GameObject go = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.WeaponSwing);
        go.transform.position = weapon.attackPoint.transform.position;
        //go.transform.parent = meleeWeapon.attackPoint; // Decide which feels better. Stay in place or follow sword
        go.transform.up = -weapon.item.owner.GetComponent<BaseBrain>().lookingDirection;

        go.SetActive(true);
    }

    void InteruptCombo() {
        if (inCombo) {
            inCombo = false;

            weapon.item.owner.objectStatusHandler.UnblockMovementControls();

            //weapon.animator.Play("Sword_Idle");//
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Default")) return;

        ObjectHandler otherObjectHandler;

        if (other.TryGetComponent<ObjectHandler>(out otherObjectHandler)) {
            if (otherObjectHandler == weapon.item.owner) return;
            
            weapon.damageType.DealDamage(otherObjectHandler, lastCharge);
        }
    }
}
