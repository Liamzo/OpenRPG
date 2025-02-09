using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnParry Stun", menuName = "Strategies/OnParry Stun")]
public class StrategyOnParryStun : BaseStrategy
{
    public float stunDuration;


    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);

        weapon.item.OnEquip += OnEquip;
        weapon.item.OnUnequip += OnUnequip;

        if (weapon.item.owner != null && weapon.item.owner is CharacterHandler) {
            OnEquip();
        }
    }

    public override void Delete()
    {
        weapon.item.OnEquip -= OnEquip;
        weapon.item.OnUnequip -= OnUnequip;

        if (weapon.item.owner != null && weapon.item.owner is CharacterHandler) {
            OnUnequip();
        }
    }


    void OnEquip() {
        ((CharacterHandler)weapon.item.owner).OnParry += DoStun;
    }
    void OnUnequip() {
        ((CharacterHandler)weapon.item.owner).OnParry -= DoStun;
    }


    private void DoStun(WeaponHandler weapon, ObjectHandler damageDealer, GameObject projectile)
    {
        damageDealer.objectStatusHandler.BlockControls(stunDuration); // Move somewhere else. Probably make an Effect Handler for things like Stunned, etc. that handles setting the objectStatusHandler
        damageDealer.objectStatusHandler.BlockMovementControls(stunDuration); // Move somewhere else
    }
}
