using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardBullet : MonoBehaviour
{
    public float bulletSpeed;
    public Vector3 direction;
    public BaseWeaponHandler weapon;

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

    private void OnTriggerEnter2D(Collider2D other) {
        //if (other.gameObject.layer != LayerMask.NameToLayer("Default")) return;

        ObjectHandler objectHandler;

        if (other.TryGetComponent<ObjectHandler>(out objectHandler)) {
            objectHandler.GetHit(weapon, (CharacterHandler)weapon.item.owner);
        }

        Destroy(gameObject);
    }
}
