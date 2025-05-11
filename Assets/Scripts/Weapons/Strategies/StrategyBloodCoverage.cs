using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BloodCoverage", menuName = "Strategies/Blood Coverage")]
public class StrategyBloodCoverage : BaseStrategy
{
    SpriteRenderer spriteRenderer;

    float bloodAmount = 0f;


    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        spriteRenderer = weapon.item.objectHandler.spriteRenderer;

        weapon.triggerHolders[triggerSlot].OnHitTarget += HitTarget;
        weapon.triggerHolders[triggerSlot].OnAttack += FlingBlood;
    }

    public override void Delete()
    {
        weapon.triggerHolders[triggerSlot].OnHitTarget -= HitTarget;
        weapon.triggerHolders[triggerSlot].OnAttack -= FlingBlood;
    }

    // On Hit Target
    private void HitTarget(ObjectHandler target, HitOutcome hitOutcome, float charge, BasicBullet projectile)
    {
        if (target is not CharacterHandler || hitOutcome != HitOutcome.Hit) return;

        bloodAmount = Mathf.Clamp(bloodAmount + 0.35f, 0f, 1f);

        float bloodCoverage = Mathf.Lerp(0f, 0.65f, bloodAmount);
        spriteRenderer.material.SetFloat("_DissolveCoverage", bloodCoverage);
    }

    // On Attack
    private void FlingBlood()
    {
        bloodAmount = Mathf.Clamp(bloodAmount - 0.2f, 0f, 1f);

        float bloodCoverage = Mathf.Lerp(0f, 0.65f, bloodAmount);
        spriteRenderer.material.SetFloat("_DissolveCoverage", bloodCoverage);
    }
}
