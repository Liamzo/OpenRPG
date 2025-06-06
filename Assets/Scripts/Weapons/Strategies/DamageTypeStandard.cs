using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DamageType Standard", menuName = "Strategies/DamageType Standard")]
public class DamageTypeStandard : BaseStrategy, IDamageType
{
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnHitTarget += DealDamage;
    }

    public override void Delete()
    {
        weapon.triggerHolders[triggerSlot].OnHitTarget -= DealDamage;
    }

    public void DealDamage(ObjectHandler target, HitOutcome hitOutcome, float charge, BasicBullet projectile)
    {
        // Roll for Damage
        float damage = ((CharacterHandler) weapon.item.owner).Attributes.GetAttribute(AttributeNames.Strength);

        for (int i = 0; i < weapon.GetStatValue(WeaponStatNames.DamageRollCount); i++) {
            damage += Random.Range(1, (int)weapon.GetStatValue(WeaponStatNames.DamageRollValue) + 1);
        }

        if (hitOutcome == HitOutcome.Hit) {
            GameManager.instance.HitStop(0.05f); // Move to future BaseDamage class

            if (target != null){
                target.objectStatusHandler.BlockControls(weapon.GetStatValue(WeaponStatNames.Stagger)); // Move somewhere else. Probably make an Effect Handler for things like Stunned, etc. that handles setting the objectStatusHandler
                target.objectStatusHandler.BlockMovementControls(weapon.GetStatValue(WeaponStatNames.Stagger)); // Move somewhere else

                target.TakeDamge(damage, weapon, (CharacterHandler) weapon.item.owner, projectile); // Move to future BaseDamage class

                if (target is CharacterHandler) {
                    ((CharacterHandler)target).ChangeStamina(-damage); // Move somewhere else
                }
            }

        } else if (hitOutcome == HitOutcome.Block) {
            //GameManager.instance.HitStop(0.05f); // Move to future BaseDamage class

            if (target != null){
                target.objectStatusHandler.BlockMovementControls(weapon.GetStatValue(WeaponStatNames.Stagger)/2f);

                if (target is CharacterHandler) {
                    ((CharacterHandler)target).ChangeStamina(-damage/2f);
                }
            }
        } else if (hitOutcome == HitOutcome.Dodge) {

        }

    }
}
