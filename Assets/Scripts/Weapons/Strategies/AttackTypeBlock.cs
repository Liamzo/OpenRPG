using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeBlock : BaseStrategy, IAttackType
{
    private void Start() {
        weapon.triggerHolders[triggerSlot].OnTrigger += DoAttack;
        weapon.triggerHolders[triggerSlot].OnTriggerRelease += ReleaseBlock;
    }

    public void DoAttack(float charge)
    {
        weapon.animator.Play("Combo_Block");

        Debug.Log(weapon.item.owner.GetComponent<BaseBrain>().lookingDirection);
    }

    void ReleaseBlock(float charge) {
        if (weapon.CanAttack()) {
            weapon.animator.SetTrigger("Idle");
            weapon.animator.speed = 1.0f;
        }
    }
}
