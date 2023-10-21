using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTypeShotgun : BaseStrategy, IAttackType
{
    public MuzzleFlash muzzleFlash;
    public ParticleSystem fireEffect;
    ParticleSystem.EmissionModule fireEffectEmission;

    private void Start() {
        fireEffectEmission = fireEffect.emission;
    }

    public void DoAttack()
    {
        List<ObjectHandler> targets = EnemiesInCone();

        foreach(ObjectHandler target in targets) {
            target.GetHit(weapon, (CharacterHandler)weapon.item.owner); // Could potentially delay based on distance
        }

        fireEffect.Play();
        muzzleFlash.Flash();
    }

    private void Update() {
        fireEffect.transform.localEulerAngles = new Vector3(0f, weapon.item.objectHandler.spriteRenderer.transform.eulerAngles.y, 60f);
    }

    List<ObjectHandler> EnemiesInCone() {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, weapon.statsWeapon[WeaponStatNames.Range].GetValue()); // Get all possible objects in range
        List<ObjectHandler> enemyStats = new List<ObjectHandler>();

        foreach (Collider2D col in objects) {
            ObjectHandler objectHandler = col.GetComponent<ObjectHandler>();

            if (objectHandler != null) {
                if (objectHandler == weapon.item.owner)
                    continue;

                float dist = Vector3.Distance(objectHandler.transform.position, transform.position);

                Vector2 dir = (objectHandler.transform.position - transform.position).normalized;

                if (dist <= weapon.statsWeapon[WeaponStatNames.Range].GetValue() && (Vector3.Angle(-transform.up, dir) <= 90f / 2f || dist < 0.5f)) {
                    enemyStats.Add(objectHandler);
                }
            }
        }

        return enemyStats;
    }
}
