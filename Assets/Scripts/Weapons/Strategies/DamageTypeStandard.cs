using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTypeStandard : BaseStrategy, IDamageType
{
    void Start()
    {
        weapon.triggerHolders[triggerSlot].OnHitTarget += DealDamage;
    }

    public void DealDamage(ObjectHandler target, HitOutcome hitOutcome, float charge, GameObject projectile)
    {
        // Roll for Damage
        float damage = ((CharacterHandler) weapon.item.owner).Attributes.GetAttribute(AttributeNames.Strength);

        for (int i = 0; i < weapon.GetStatValue(WeaponStatNames.DamageRollCount); i++) {
            damage += Random.Range(1, (int)weapon.GetStatValue(WeaponStatNames.DamageRollValue) + 1);
        }

        if (hitOutcome == HitOutcome.Hit) {
            GameManager.instance.ShakeCamera(8.0f, 0.2f); // Move to future BaseDamage class
            GameManager.instance.HitStop(0.05f); // Move to future BaseDamage class

            if (target != null){
                target.objectStatusHandler.BlockControls(weapon.GetStatValue(WeaponStatNames.Stagger)); // Move somewhere else
                target.objectStatusHandler.BlockMovementControls(weapon.GetStatValue(WeaponStatNames.Stagger)); // Move somewhere else

                target.TakeDamge(damage, weapon, (CharacterHandler) weapon.item.owner); // Move to future BaseDamage class

                CharacterHandler targetCharacter = (CharacterHandler) target;
                if (targetCharacter != null) {
                    targetCharacter.ChangeStamina(-damage); // Move somewhere else
                }
            }

        } else if (hitOutcome == HitOutcome.Block) {
            GameManager.instance.ShakeCamera(5.0f, 0.15f); // Move to future BaseDamage class
            GameManager.instance.HitStop(0.05f); // Move to future BaseDamage class

            if (target != null){
                target.objectStatusHandler.BlockMovementControls(weapon.GetStatValue(WeaponStatNames.Stagger)/2f);

                CharacterHandler targetCharacter = (CharacterHandler) target;
                if (targetCharacter != null) {
                    targetCharacter.ChangeStamina(-damage/2f);
                }
            }
        } else if (hitOutcome == HitOutcome.Dodge) {

        }

    }
}
