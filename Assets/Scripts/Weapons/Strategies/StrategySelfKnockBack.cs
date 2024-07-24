using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategySelfKnockBack : BaseStrategy
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

    private void DoKnockBack()
    {
        weapon.item.owner.GetComponent<Physicsable>().Knock(-transform.up, weapon.statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());
    }

    void DoKnockBack(float charge) {
        DoKnockBack();
    }

    private void DoKnockBack(ObjectHandler target, HitOutcome hitOutcome, float charge, GameObject projectile)
    {
        DoKnockBack();
    }
}
