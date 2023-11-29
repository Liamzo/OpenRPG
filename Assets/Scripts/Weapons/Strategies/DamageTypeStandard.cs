using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTypeStandard : BaseStrategy, IDamageType
{
    public void DealDamage(ObjectHandler target, float charge)
    {
        // Roll for Damage
        float damage = 0.0f;

        for (int i = 0; i < weapon.statsWeapon[WeaponStatNames.DamageRollCount].GetValue(); i++) {
            damage += Random.Range(1, (int)weapon.GetStatValue(WeaponStatNames.DamageRollValue) + 1);
        }

        if (target.GetHit(damage, weapon, (CharacterHandler) weapon.item.owner)) {
            GameManager.instance.ShakeCamera(5.0f, 0.15f);
            GameManager.instance.HitStop(0.1f);

            target.objectStatusHandler.BlockControls(weapon.GetStatValue(WeaponStatNames.Stagger));
            target.objectStatusHandler.BlockMovementControls(weapon.GetStatValue(WeaponStatNames.Stagger));
        }
    }
}
