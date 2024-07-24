using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitTargetSound : BaseStrategy
{
    [SerializeField] private AudioID audioID;

    AudioSource weaponAudioSource;

    // Start is called before the first frame update
    public override void Create() {
        base.Create();
        
        weapon.triggerHolders[triggerSlot].OnHitTarget += PlaySound;

        weaponAudioSource = weapon.GetComponent<AudioSource>();
    }

    void PlaySound(ObjectHandler target, HitOutcome hitOutcome, float charge, GameObject projectile) {
        if (hitOutcome == HitOutcome.Hit) {
            AudioManager.instance.PlayClipRandom(audioID, weaponAudioSource);
        } else if (hitOutcome == HitOutcome.Block) {
            AudioManager.instance.PlayClipRandom(AudioID.Block, weaponAudioSource);
        }
    }
}
