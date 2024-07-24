using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeBlock : BaseStrategy, IAttackType
{
    bool isBlocking = false;

    public float blockAngle;


    public override void Create() {
        base.Create();
        
        weapon.triggerHolders[triggerSlot].OnTrigger += DoAttack;
        weapon.triggerHolders[triggerSlot].OnTriggerRelease += ReleaseBlock;
        weapon.triggerHolders[triggerSlot].OnAttackCancel += ReleaseBlock;
        weapon.item.OnEquip += OnEquip;
        weapon.item.OnUnequip += OnUnequip;
    }

    public void DoAttack(float charge)
    {
        if (!isBlocking) {
            isBlocking = true;
            weapon.item.owner.objectStatusHandler.Block(blockAngle);

            weapon.animator.Play("Combo_Block");
        }
    }


    void ReleaseBlock() {
        if (isBlocking) {
            isBlocking = false;
            weapon.item.owner.objectStatusHandler.StopBlock();

            if (weapon.CanAttack()) {
                weapon.animator.SetTrigger("Idle");
                weapon.animator.speed = 1.0f;
            }
        }
    }

    void ReleaseBlock(float charge) {
        ReleaseBlock();
    }
    void ReleaseBlock(float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        ReleaseBlock();
    }


    void OnEquip() {
        weapon.item.owner.OnTakeDamage += ReleaseBlock;
    }

    void OnUnequip() {
        weapon.item.owner.OnTakeDamage -= ReleaseBlock;
    }
}
