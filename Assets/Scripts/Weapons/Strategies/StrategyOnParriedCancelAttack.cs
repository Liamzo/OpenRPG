using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "On Parried Cancel Attack", menuName = "Strategies/On Parried Cancel Attack")]
public class StrategyOnParriedCancelAttack : BaseStrategy
{
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnHitTarget += DoCancelAttack;
    }

    public override void Delete()
    {
        weapon.triggerHolders[triggerSlot].OnHitTarget -= DoCancelAttack;
    }


    private void DoCancelAttack(ObjectHandler target, HitOutcome hitOutcome, float charge, BasicBullet projectile)
    {
        if (hitOutcome != HitOutcome.Parry) return;

        weapon.RunCoroutine(ReverseAnimation(weapon));
    }


    IEnumerator ReverseAnimation(WeaponHandler weapon) {
        weapon.item.owner.objectStatusHandler.BlockControls(0.2f); // Move somewhere else. Probably make an Effect Handler for things like Stunned, etc. that handles setting the objectStatusHandler
        weapon.item.owner.objectStatusHandler.BlockMovementControls(0.2f); // Move somewhere else

        float startPlaybackTime = weapon.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float elapsedTime = 0f;

        while (true) {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Lerp(startPlaybackTime, 0.05f, elapsedTime / 0.2f);

            weapon.animator.Play(weapon.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, 0, normalizedTime); // Update animation playback
            weapon.animator.speed = 0; // Stop automatic animation speed control

            if (elapsedTime >= 0.2f)
            {
                //weapon.animator.speed = 1; // Reset to normal speed
                break;
            }

            yield return null;
        }

        weapon.AttackCancel(triggerSlot);
    }
}
