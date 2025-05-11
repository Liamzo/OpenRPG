using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Self Knockback", menuName = "Strategies/Self Knockback")]
public class StrategySelfKnockBack : BaseStrategy
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

    private void DoKnockBack()
    {
        weapon.item.owner.GetComponent<Physicsable>().Knock(-weapon.transform.up, weapon.statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());
    }

    void DoKnockBack(float charge) {
        DoKnockBack();
    }

    private void DoKnockBack(ObjectHandler target, HitOutcome hitOutcome, float charge, BasicBullet projectile)
    {
        DoKnockBack();
    }
}
