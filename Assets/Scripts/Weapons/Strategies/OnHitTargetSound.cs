using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitTargetSound : BaseStrategy
{
    [SerializeField] private AudioID audioID;

    AudioSource weaponAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        weapon.triggerHolders[triggerSlot].OnHitTarget += PlaySound;

        weaponAudioSource = weapon.GetComponent<AudioSource>();
    }

    void PlaySound(ObjectHandler target, HitOutcome hitOutcome, float charge) {
        AudioManager.instance.PlayClipRandom(audioID, weaponAudioSource);
    }
}
