using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttckTypeSlash : BaseStrategy, IAttackType
{
    private void Start() {
        weapon.OnTrigger += DoAttack;
        weapon.OnAttack += SpawnSlashAnimEvent;
    }

    public void DoAttack()
    {
        weapon.animator.SetTrigger("Attack");
    }

    public void SpawnSlashAnimEvent() {
        GameObject go = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.WeaponSwing);
        go.transform.position = weapon.attackPoint.transform.position;
        //go.transform.parent = meleeWeapon.attackPoint; // Decide which feels better. Stay in place or follow sword
        go.transform.up = weapon.item.owner.GetComponent<BaseBrain>().lookingDirection;

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
