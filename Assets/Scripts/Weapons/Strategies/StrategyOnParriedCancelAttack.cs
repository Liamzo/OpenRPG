using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "On Parried Cancel Attack", menuName = "Strategies/On Parried Cancel Attack")]
public class StrategyOnParriedCancelAttack : BaseStrategy
{
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnHitTarget += DoCancelAttack;
    }

    public override void Delete()
    {
        weapon.triggerHolders[triggerSlot].OnHitTarget -= DoCancelAttack;
    }


    private void DoCancelAttack(ObjectHandler target, HitOutcome hitOutcome, float charge, GameObject projectile)
    {
        if (hitOutcome != HitOutcome.Parry) return;

        weapon.AttackCancel(triggerSlot);
        weapon.item.owner.objectStatusHandler.BlockControls(1f); // Move somewhere else. Probably make an Effect Handler for things like Stunned, etc. that handles setting the objectStatusHandler
        weapon.item.owner.objectStatusHandler.BlockMovementControls(1f); // Move somewhere else
    }
}
