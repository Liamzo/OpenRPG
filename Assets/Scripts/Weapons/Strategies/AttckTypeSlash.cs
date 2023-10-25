using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttckTypeSlash : BaseStrategy, IAttackType
{
    protected override void Awake() {
        base.Awake();
        
        meleeWeapon.OnAttack += SpawnSlashAnimEvent;
    }

    public void DoAttack()
    {
        meleeWeapon.animator.SetTrigger("Attack");
        //attackTimer = statsWeapon[WeaponStatNames.AttackTimer].GetValue(); // Do fire rate for melee
    }

    public void SpawnSlashAnimEvent() {
        GameObject go = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.WeaponSwing);
        go.transform.position = meleeWeapon.attackPoint.transform.position;
        //go.transform.parent = meleeWeapon.attackPoint; // Decide which feels better. Stay in place or follow sword
        go.transform.up = meleeWeapon.item.owner.GetComponent<BaseBrain>().lookingDirection;

        go.SetActive(true);
    }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     if (other.gameObject.layer != LayerMask.NameToLayer("Default")) return;

    //     ObjectHandler otherObjectHandler;

    //     if (other.TryGetComponent<ObjectHandler>(out otherObjectHandler)) {
    //         if (otherObjectHandler == meleeWeapon.item.owner) return;
            
    //         meleeWeapon.damageType.DealDamage(otherObjectHandler);
    //     }
    // }
}
