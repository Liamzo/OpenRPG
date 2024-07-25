using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Strategies/Sound")]
public class StrategySound : BaseStrategy
{
    [SerializeField] WeaponEvents triggerEvent;

    [SerializeField] private AudioID audioID;

    AudioSource weaponAudioSource;

    // Start is called before the first frame update
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);

        weaponAudioSource = weapon.GetComponent<AudioSource>();

        switch (triggerEvent)
        {
            case WeaponEvents.OnAttack:
                weapon.triggerHolders[triggerSlot].OnAttack += PlaySound;
                break;
            case WeaponEvents.OnHitTarget:
                weapon.triggerHolders[triggerSlot].OnHitTarget += PlaySound;
                break;
            case WeaponEvents.OnTrigger:
                weapon.triggerHolders[triggerSlot].OnTrigger += PlaySound;
                break;
            case WeaponEvents.OnTriggerRelease:
                weapon.triggerHolders[triggerSlot].OnTriggerRelease += PlaySound;
                break;
        }
    }

    // On Attack
    private void PlaySound()
    {
        AudioManager.instance.PlayClipRandom(audioID, weaponAudioSource);
    }

    // On Trigger and Release
    void PlaySound(float charge) {
        PlaySound();
    }

    // On Hit Target
    private void PlaySound(ObjectHandler target, HitOutcome hitOutcome, float charge, GameObject projectile)
    {
        if (target == null) return;
        
        PlaySound();
    }
}
