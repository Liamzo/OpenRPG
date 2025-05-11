using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public float bulletSpeed;
    public float weaponCharge;
    public Vector3 direction;
    public WeaponHandler weapon;
    public int triggerSlot;

    public float bulletLifeTime;
    float bulletLifeTimer;

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * bulletSpeed * Time.deltaTime;

        bulletLifeTimer += Time.deltaTime;
        if  (bulletLifeTimer >= bulletLifeTime) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.TryGetComponent<ObjectHandler>(out ObjectHandler objectHandler)) {
            if (objectHandler == weapon.item.owner) return;

            weapon.CallOnHitTarget(triggerSlot, objectHandler, weaponCharge, this);
        } else {
            weapon.CallOnHitTarget(triggerSlot, null, weaponCharge, this);
        }

        Destroy(gameObject);
    }
}
