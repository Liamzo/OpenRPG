using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyKnockBack : BaseStrategy
{
    [SerializeField] WeaponEvents triggerEvent;

    public override void Create() {
        base.Create();
        
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
    private void DoKnockBack(ObjectHandler target, HitOutcome hitOutcome, float charge, GameObject projectile)
    {
        if (target == null) return;
        
        Vector3 hitPosition = projectile == null ? weapon.item.owner.transform.position : projectile.transform.position;

        if (hitOutcome == HitOutcome.Hit) {
            target.GetComponent<Physicsable>().KnockBack(hitPosition, weapon.statsWeapon[WeaponStatNames.KnockBack].GetValue());
        } else if (hitOutcome == HitOutcome.Block) {
            target.GetComponent<Physicsable>().KnockBack(hitPosition, weapon.statsWeapon[WeaponStatNames.KnockBack].GetValue()/2f);
        }
    }
}
