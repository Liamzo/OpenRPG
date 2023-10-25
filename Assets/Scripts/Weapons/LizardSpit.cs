using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardSpit : WeaponHandler
{
    float attackTimer = 0f;

    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public float bulletSpeed;

    // Update is called once per frame
    protected void Update()
    {
        if (attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }
    }
}
