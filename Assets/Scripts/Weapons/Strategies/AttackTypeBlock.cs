using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeBlock : BaseStrategy, IAttackType
{
    bool isBlocking = false;

    public float blockAngle;


    private void Start() {
        weapon.triggerHolders[triggerSlot].OnTrigger += DoAttack;
        weapon.triggerHolders[triggerSlot].OnTriggerRelease += ReleaseBlock;
        weapon.item.owner.OnTakeDamage += ReleaseBlock;
    }

    public void DoAttack(float charge)
    {
        if (!isBlocking) {
            isBlocking = true;
            weapon.item.owner.objectStatusHandler.Block(blockAngle);

            weapon.animator.Play("Combo_Block");
        }
    }

    void ReleaseBlock(float charge) {
        if (isBlocking) {
            isBlocking = false;
            weapon.item.owner.objectStatusHandler.StopBlock();

            if (weapon.CanAttack()) {
                weapon.animator.SetTrigger("Idle");
                weapon.animator.speed = 1.0f;
            }
        }
    }
    void ReleaseBlock(float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        ReleaseBlock(0f);
    }
}
