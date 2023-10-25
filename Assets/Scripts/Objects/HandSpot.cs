using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandSpot : MonoBehaviour
{
    Transform owner;
    public float offset;
    public WeaponHandler weapon;

    // Start is called before the first frame update
    void Start()
    {
        owner = transform.parent;
    }

    void Update() {
        if (weapon != null) {
            SpriteRenderer sprite = weapon.item.objectHandler.spriteRenderer;


            // if (transform.localPosition.y > 0) {
            //     sprite.sortingOrder = -1;
            // } else if (transform.localPosition.y < 0) {
            //     sprite.sortingOrder = 1;
            // }

            if (sprite.transform.position.y > transform.parent.position.y) {
                sprite.sortingOrder = -1;
            } else if (sprite.transform.position.y < transform.parent.position.y) {
                sprite.sortingOrder = 1;
            }
        }
    }

    public void AimAtTarget(Vector3 target, EquipmentSlot slot) {
        Vector2 diff = target - owner.position;
        diff = diff.normalized;

        Vector2 spotPos = Vector2.Perpendicular(diff) * -1;

        if (slot == EquipmentSlot.RightHand) {
            spotPos = Vector2.Perpendicular(diff) * -1;
        } else if (slot == EquipmentSlot.LeftHand) {
            spotPos = Vector2.Perpendicular(diff);
        } else if (slot == EquipmentSlot.RightRangedWeapon) {
            spotPos = (diff + new Vector2(0.5f, 0.0f)).normalized;
        } else if (slot == EquipmentSlot.LeftRangedWeapon) {
            spotPos = (diff + new Vector2(-0.5f, 0.0f)).normalized;
        } 

        transform.localPosition = spotPos * offset;
        transform.up = diff * -1;
    }
}
