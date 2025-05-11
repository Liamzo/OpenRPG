using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Camera Shake", menuName = "Strategies/Camera Shake")]
public class StrategyCameraShake : BaseStrategy
{
    [SerializeField] WeaponEvents triggerEvent;

    
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        switch (triggerEvent)
        {
            case WeaponEvents.OnAttack:
                weapon.triggerHolders[triggerSlot].OnAttack += CameraShake;
                break;
            case WeaponEvents.OnHitTarget:
                weapon.triggerHolders[triggerSlot].OnHitTarget += CameraShake;
                break;
            case WeaponEvents.OnTrigger:
                weapon.triggerHolders[triggerSlot].OnTrigger += CameraShake;
                break;
            case WeaponEvents.OnTriggerRelease:
                weapon.triggerHolders[triggerSlot].OnTriggerRelease += CameraShake;
                break;
        }
    }

    public override void Delete()
    {
        switch (triggerEvent)
        {
            case WeaponEvents.OnAttack:
                weapon.triggerHolders[triggerSlot].OnAttack -= CameraShake;
                break;
            case WeaponEvents.OnHitTarget:
                weapon.triggerHolders[triggerSlot].OnHitTarget -= CameraShake;
                break;
            case WeaponEvents.OnTrigger:
                weapon.triggerHolders[triggerSlot].OnTrigger -= CameraShake;
                break;
            case WeaponEvents.OnTriggerRelease:
                weapon.triggerHolders[triggerSlot].OnTriggerRelease -= CameraShake;
                break;
        }
    }


    // On Attack
    private void CameraShake()
    {
        CameraShake(1f);
    }

    // On Trigger and Release
    void CameraShake(float charge) {
        GameManager.instance.ShakeCamera(5.0f, 0.15f);
    }

    // On Hit Target
    private void CameraShake(ObjectHandler target, HitOutcome hitOutcome, float charge, BasicBullet projectile)
    {
        if (target == null) return;
        
        CameraShake(1f);
    }
}
