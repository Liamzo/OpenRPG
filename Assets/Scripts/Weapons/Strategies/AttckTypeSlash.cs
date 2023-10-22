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
        // go.transform.position = item.owner.transform.position;
        go.transform.parent = meleeWeapon.transform.parent.parent; // Orbit point
        go.transform.localPosition = Vector3.zero;
        go.transform.up = meleeWeapon.item.owner.GetComponent<BaseBrain>().lookingDirection;
        go.transform.localPosition += -transform.up * 0.1f;

        go.SetActive(true);
    }
}
