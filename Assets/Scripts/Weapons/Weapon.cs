using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Weapon : BaseWeaponHandler
{
    public Collider2D hitbox;

    float attackTimer = 0f;

    bool isCharging = false;
    float chargeTimer = 0f;
    float chargeSheenTimer = -1.5f;


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        if (attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }

        if (isCharging) {
            chargeTimer += Time.deltaTime;

            if (chargeTimer > 1f) {
                chargeSheenTimer -= Time.deltaTime * 1.5f;

                item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", chargeSheenTimer);
            }
        }
        
    }

    protected override void LateUpdate() {
        if (item.owner is null)
            return;

        base.LateUpdate();

        IdleFollow();
    }

    public override float AttackHoldCost()
    {
        return statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
    }
    public override float AttackHold() {
        isCharging = true;
        animator.SetBool("Charging", true);
        return statsWeapon[WeaponStatNames.StaminaCostHold].GetValue() * Time.deltaTime;
    }

    public override float AttackEndCost()
    {
        return statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }
    public override float AttackEnd() {
        animator.SetBool("Charging", false);
        isCharging = false;
        Attack();
        chargeTimer = 0f;
        chargeSheenTimer = -1.48f;
        item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", 0f);

        item.owner.GetComponent<Physicsable>().Knock(-transform.up, statsWeapon[WeaponStatNames.SelfKnockForce].GetValue());

        return statsWeapon[WeaponStatNames.StaminaCostEnd].GetValue();
    }

    public void Attack() {
        animator.SetTrigger("Attack");
        attackTimer = statsWeapon[WeaponStatNames.AttackTimer].GetValue();
    }

    public override void AttackCancel() {
        animator.SetBool("Charging", false);
        isCharging = false;
        chargeTimer = 0f;
        chargeSheenTimer = -1.5f;
        item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", 0f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Default")) return;

        ObjectHandler otherObjectHandler;

        if (other.TryGetComponent<ObjectHandler>(out otherObjectHandler)) {
            if (otherObjectHandler == item.owner) return;
            
            otherObjectHandler.GetHit(this, (CharacterHandler) item.owner);
        }
    }

    public void SpawnSlashAnimEvent(SwingDir dir) {
        GameObject go = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.WeaponSwing);
        // go.transform.position = item.owner.transform.position;
        go.transform.parent = item.owner.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.up = item.owner.GetComponent<BaseBrain>().lookingDirection;

        if (dir == SwingDir.RtL) {
            go.transform.localScale = new Vector3(1f,1f,1f);
        } else if (dir == SwingDir.LtR) {
            go.transform.localScale = new Vector3(-1f,1f,1f);
        }

        go.SetActive(true);
    }


    public override void AttackAnticipation()
    {
        StartCoroutine(DoSheen());
    }

    public IEnumerator DoSheen() {
        float timer = 0f;
        while (timer <= 1f) {
            timer += Time.deltaTime * 5f; // 0.2 secs
            item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", Mathf.Lerp(-1.48f, -1.9f, timer));
            yield return null;
        }

        item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", 0f);
    }
}