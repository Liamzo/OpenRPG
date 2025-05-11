using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnParry Attack", menuName = "Strategies/OnParry Attack")]
public class StrategyOnParryAttack : BaseStrategy
{
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
        ((CharacterHandler)weapon.item.owner).OnParry += DoAttack;
    }
    void OnUnequip() {
        ((CharacterHandler)weapon.item.owner).OnParry -= DoAttack;
    }


    private void DoAttack(WeaponHandler attackingWeapon, ObjectHandler damageDealer, BasicBullet projectile)
    {
        weapon.AttackHold(0);
        weapon.AttackRelease(0);
    }
}
