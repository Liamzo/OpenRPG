using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerSound : BaseStrategy
{
    [SerializeField] private AudioID audioID;

    AudioSource weaponAudioSource;

    // Start is called before the first frame update
    public override void Create() {
        base.Create();
        
        weapon.triggerHolders[triggerSlot].OnTrigger += PlaySound;

        weaponAudioSource = weapon.GetComponent<AudioSource>();
    }

    void PlaySound(float charge) {
        AudioManager.instance.PlayClipRandom(audioID, weaponAudioSource);
    }
}
