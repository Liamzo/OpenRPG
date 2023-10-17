using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
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
        ObjectHandler objectStatsHandler;

        if (other.TryGetComponent<ObjectHandler>(out objectStatsHandler)) {
            objectStatsHandler.GetHit(weapon, (CharacterHandler)weapon.item.owner);
        }

        Destroy(gameObject);
    }
}
