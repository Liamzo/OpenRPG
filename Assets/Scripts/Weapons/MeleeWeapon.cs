using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class MeleeWeapon : BaseWeaponHandler
{
    protected override void LateUpdate() {
        if (item.owner is null)
            return;

        base.LateUpdate();

        IdleFollow();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Default")) return;

        ObjectHandler otherObjectHandler;

        if (other.TryGetComponent<ObjectHandler>(out otherObjectHandler)) {
            if (otherObjectHandler == item.owner) return;
            
            damageType.DealDamage(otherObjectHandler);
        }
    }


    public override void AttackAnticipation()
    {
        StartCoroutine(DoSheen());
    }

    public IEnumerator DoSheen() {
        float timer = 0f;
        while (timer <= 1f) {
            timer += Time.deltaTime * 5f; // 0.2 secs
            item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", Mathf.Lerp(0f, -2f, timer));
            yield return null;
        }

        item.objectHandler.spriteRenderer.material.SetFloat("_Sheen", 0f);
    }
}