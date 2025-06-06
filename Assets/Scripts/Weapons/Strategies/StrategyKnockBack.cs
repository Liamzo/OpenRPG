using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Knockback", menuName = "Strategies/Knockback")]
public class StrategyKnockBack : BaseStrategy
{
    [SerializeField] WeaponEvents triggerEvent;

    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        switch (triggerEvent)
        {
            case WeaponEvents.OnAttack:
                weapon.triggerHolders[triggerSlot].OnAttack += DoKnockBack;
                break;
            case WeaponEvents.OnHitTarget:
                weapon.triggerHolders[triggerSlot].OnHitTarget += DoKnockBack;
                break;
            case WeaponEvents.OnTrigger:
                weapon.triggerHolders[triggerSlot].OnTrigger += DoKnockBack;
                break;
            case WeaponEvents.OnTriggerRelease:
                weapon.triggerHolders[triggerSlot].OnTriggerRelease += DoKnockBack;
                break;
        }
    }

    public override void Delete()
    {
        switch (triggerEvent)
        {
            case WeaponEvents.OnAttack:
                weapon.triggerHolders[triggerSlot].OnAttack -= DoKnockBack;
                break;
            case WeaponEvents.OnHitTarget:
                weapon.triggerHolders[triggerSlot].OnHitTarget -= DoKnockBack;
                break;
            case WeaponEvents.OnTrigger:
                weapon.triggerHolders[triggerSlot].OnTrigger -= DoKnockBack;
                break;
            case WeaponEvents.OnTriggerRelease:
                weapon.triggerHolders[triggerSlot].OnTriggerRelease -= DoKnockBack;
                break;
        }
    }


    // On Attack
    private void DoKnockBack()
    {
        throw new System.Exception("This Strategy does not work with this event");
    }

    // On Trigger and Release
    void DoKnockBack(float charge) {
        throw new System.Exception("This Strategy does not work with this event");
    }

    // On Hit Target
    private void DoKnockBack(ObjectHandler target, HitOutcome hitOutcome, float charge, BasicBullet projectile)
    {
        if (target == null) return;
        
        Vector3 hitDirection = projectile == null ? (target.transform.position - weapon.item.owner.transform.position).normalized : projectile.direction;

        if (hitOutcome == HitOutcome.Hit) {
            target.GetComponent<Physicsable>().Knock(hitDirection, weapon.statsWeapon[WeaponStatNames.KnockBack].GetValue());
        } else if (hitOutcome == HitOutcome.Block) {
            target.GetComponent<Physicsable>().Knock(hitDirection, weapon.statsWeapon[WeaponStatNames.KnockBack].GetValue()/2f);
        }
    }
}
