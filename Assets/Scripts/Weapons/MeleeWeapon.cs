using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class MeleeWeapon : BaseWeaponHandler
{
    // private void OnTriggerEnter2D(Collider2D other) {
    //     if (other.gameObject.layer != LayerMask.NameToLayer("Default")) return;

    //     ObjectHandler otherObjectHandler;

    //     if (other.TryGetComponent<ObjectHandler>(out otherObjectHandler)) {
    //         if (otherObjectHandler == item.owner) return;
            
    //         damageType.DealDamage(otherObjectHandler);
    //     }
    // }
}