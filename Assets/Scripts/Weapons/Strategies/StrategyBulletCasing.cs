using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BulletCasing", menuName = "Strategies/Bullet Casing")]
public class StrategyBulletCasing : BaseStrategy
{
    [SerializeField] WeaponEvents triggerEvent;
    [SerializeField] GameObject particleSystemPrefab;
    ParticleSystem particleSystem;

    [SerializeField] Vector3 spawnPosition;


    public override void Create(WeaponHandler weapon) {
        base.Create(weapon);
        
        switch (triggerEvent)
        {
            case WeaponEvents.OnAttack:
                weapon.triggerHolders[triggerSlot].OnAttack += SpawnBulletCasing;
                break;
            case WeaponEvents.OnHitTarget:
                weapon.triggerHolders[triggerSlot].OnHitTarget += SpawnBulletCasing;
                break;
            case WeaponEvents.OnTrigger:
                weapon.triggerHolders[triggerSlot].OnTrigger += SpawnBulletCasing;
                break;
            case WeaponEvents.OnTriggerRelease:
                weapon.triggerHolders[triggerSlot].OnTriggerRelease += SpawnBulletCasing;
                break;
        }

        GameObject go = Instantiate(particleSystemPrefab, weapon.transform);
        go.transform.localPosition = spawnPosition;
        particleSystem = go.GetComponent<ParticleSystem>();
    }

    public override void Delete()
    {
        switch (triggerEvent)
        {
            case WeaponEvents.OnAttack:
                weapon.triggerHolders[triggerSlot].OnAttack -= SpawnBulletCasing;
                break;
            case WeaponEvents.OnHitTarget:
                weapon.triggerHolders[triggerSlot].OnHitTarget -= SpawnBulletCasing;
                break;
            case WeaponEvents.OnTrigger:
                weapon.triggerHolders[triggerSlot].OnTrigger -= SpawnBulletCasing;
                break;
            case WeaponEvents.OnTriggerRelease:
                weapon.triggerHolders[triggerSlot].OnTriggerRelease -= SpawnBulletCasing;
                break;
        }
    }

    // On Attack
    private void SpawnBulletCasing()
    {
        particleSystem.Emit(1);
    }

    // On Trigger and Release
    void SpawnBulletCasing(float charge) {
        SpawnBulletCasing();
    }

    // On Hit Target
    private void SpawnBulletCasing(ObjectHandler target, HitOutcome hitOutcome, float charge, GameObject projectile)
    {
        SpawnBulletCasing();
    }
}
