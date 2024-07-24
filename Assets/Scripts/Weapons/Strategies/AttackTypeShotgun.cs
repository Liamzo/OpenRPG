using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTypeShotgun : BaseStrategy, IAttackType
{
    [SerializeField] private GameObject muzzleFlashPrefab;
    MuzzleFlash muzzleFlash;

    [SerializeField] private GameObject fireEffectPrefab;
    ParticleSystem fireEffect;

    public override void Create() {
        base.Create();

        muzzleFlash = Instantiate(muzzleFlashPrefab, weapon.attackPoint.transform).GetComponent<MuzzleFlash>();
        fireEffect = Instantiate(fireEffectPrefab, weapon.attackPoint.transform).GetComponent<ParticleSystem>();

        weapon.triggerHolders[triggerSlot].OnTrigger += DoAttack;
    }

    public void DoAttack(float charge)
    {
        List<ObjectHandler> targets = EnemiesInCone();

        foreach(ObjectHandler target in targets) {
            weapon.CallOnHitTarget(triggerSlot, target, charge); // Could potentially delay based on distance
        }

        fireEffect.Play();
        muzzleFlash.Flash();
        weapon.CallOnAttack(triggerSlot);
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

                if (dist <= weapon.statsWeapon[WeaponStatNames.Range].GetValue() && (Vector3.Angle(-transform.up, dir) <= 90f / 2f || dist < 1f)) {
                    enemyStats.Add(objectHandler);
                }
            }
        }

        return enemyStats;
    }
}
