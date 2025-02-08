using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "On Hit Audio", menuName = "Strategies/On Hit Audio")]
public class StrategyOnHitAudio : BaseStrategy
{
    AudioSource weaponAudioSource;


    [SerializeField] private AudioID onHitAudioID;
    [SerializeField] private AudioID onBlockAudioID;
    [SerializeField] private AudioID onDodgeAudioID;
    [SerializeField] private AudioID onParryAudioID;

    [SerializeField] private bool onHitPlay;
    [SerializeField] private bool onBlockPlay;
    [SerializeField] private bool onDodgePlay;
    [SerializeField] private bool onParryPlay;


    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);

        weaponAudioSource = weapon.GetComponent<AudioSource>();

        weapon.triggerHolders[triggerSlot].OnHitTarget += PlaySound;
    }

    public override void Delete()
    {
        weapon.triggerHolders[triggerSlot].OnHitTarget -= PlaySound;
    }


    private void PlaySound(ObjectHandler target, HitOutcome hitOutcome, float charge, GameObject projectile)
    {
        if (target == null) return;
        
        if (hitOutcome == HitOutcome.Hit && onHitPlay) {
            AudioManager.instance.PlayClipRandom(onHitAudioID, weaponAudioSource);
        } else if (hitOutcome == HitOutcome.Block && onBlockPlay) {
            AudioManager.instance.PlayClipRandom(onBlockAudioID, weaponAudioSource);
        } else if (hitOutcome == HitOutcome.Dodge && onDodgePlay) {
            AudioManager.instance.PlayClipRandom(onDodgeAudioID, weaponAudioSource);
        } else if (hitOutcome == HitOutcome.Parry && onParryPlay) {
            AudioManager.instance.PlayClipRandom(onParryAudioID, weaponAudioSource);
        }
    }
}
