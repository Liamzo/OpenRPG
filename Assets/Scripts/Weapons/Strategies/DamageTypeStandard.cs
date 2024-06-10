using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTypeStandard : BaseStrategy, IDamageType
{
    void Start()
    {
        weapon.triggerHolders[triggerSlot].OnHitTarget += DealDamage;
    }

    public void DealDamage(ObjectHandler target, HitOutcome hitOutcome, float charge)
    {
        // Roll for Damage
        float damage = 0.0f;

        for (int i = 0; i < weapon.statsWeapon[WeaponStatNames.DamageRollCount].GetValue(); i++) {
            damage += Random.Range(1, (int)weapon.GetStatValue(WeaponStatNames.DamageRollValue) + 1);
        }

        if (hitOutcome == HitOutcome.Hit) {
            GameManager.instance.ShakeCamera(8.0f, 0.2f);
            GameManager.instance.HitStop(0.05f);

            if (target != null){
                target.objectStatusHandler.BlockControls(weapon.GetStatValue(WeaponStatNames.Stagger));
                target.objectStatusHandler.BlockMovementControls(weapon.GetStatValue(WeaponStatNames.Stagger));

                target.TakeDamge(damage, weapon, (CharacterHandler) weapon.item.owner);

                CharacterHandler targetCharacter = (CharacterHandler) target;
                if (targetCharacter != null){
                    targetCharacter.ChangeStamina(-damage);
                }
            }

        } else if (hitOutcome == HitOutcome.Block) {
            GameManager.instance.ShakeCamera(5.0f, 0.15f);
            GameManager.instance.HitStop(0.05f);

            if (target != null){
                target.objectStatusHandler.BlockMovementControls(weapon.GetStatValue(WeaponStatNames.Stagger));

                CharacterHandler targetCharacter = (CharacterHandler) target;
                if (targetCharacter != null){
                    targetCharacter.ChangeStamina(-damage/2f);
                }
            }
        } else if (hitOutcome == HitOutcome.Dodge) {

        }

    }
}
