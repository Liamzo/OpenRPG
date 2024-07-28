using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackType Slash", menuName = "Strategies/AttackType Slash")]
public class AttackTypeSlash : BaseStrategy, IAttackType
{
    float lastCharge = 1f;
    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        weapon.triggerHolders[triggerSlot].OnTriggerRelease += DoAttack;
        weapon.triggerHolders[triggerSlot].OnAttack += SpawnSlashAnimEvent;

        weapon.meleeHitbox.GetComponent<WeaponHitDetector>().TriggerEntered += OnTriggerEnter2D;
    }

    public void DoAttack(float charge)
    {
        //weapon.animator.SetTrigger("Attack");
        weapon.animator.Play("Sword_Attack01");
        lastCharge = charge;
    }

    public void SpawnSlashAnimEvent() {
        GameObject go = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.WeaponSwing);
        go.transform.position = weapon.attackPoint.transform.position;
        //go.transform.parent = meleeWeapon.attackPoint; // Decide which feels better. Stay in place or follow sword
        go.transform.up = weapon.item.owner.GetComponent<BaseBrain>().lookingDirection;

        go.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Default")) return;

        ObjectHandler otherObjectHandler;

        if (other.TryGetComponent<ObjectHandler>(out otherObjectHandler)) {
            if (otherObjectHandler == weapon.item.owner) return;
            
            weapon.CallOnHitTarget(triggerSlot, otherObjectHandler, lastCharge);
        }
    }
}
