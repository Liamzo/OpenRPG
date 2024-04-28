using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfKnockBack : BaseStrategy
{
    private void Start() {
        weapon.OnPrimaryTriggerRelease += DoKnockBack;
    }

    void DoKnockBack(float charge) {
        weapon.item.owner.GetComponent<Physicsable>().Knock(-transform.up, weapon.statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());
    }
}
