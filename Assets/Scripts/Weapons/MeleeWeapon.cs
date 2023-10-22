using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class MeleeWeapon : BaseWeaponHandler
{
    public ITrigger trigger;
    public IAttackType attackType;
    public IDamageType damageType;


    // Events
    public event System.Action OnAttack = delegate { };
    public void CallOnAttack() {
        OnAttack();
    }

    protected override void Awake()
    {
        base.Awake();

        // Handle cases where no Strategy is assigned
        trigger = strategies.GetComponent<ITrigger>();
        attackType = strategies.GetComponent<IAttackType>();
        damageType = strategies.GetComponent<IDamageType>();
    }


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate() {
        if (item.owner is null)
            return;

        base.LateUpdate();

        IdleFollow();
    }

    public override float AttackHoldCost()
    {
        return trigger.AttackHoldCost();
    }
    public override float AttackHold() {
        return trigger.AttackHold();
    }

    public override float AttackReleaseCost()
    {
        return trigger.AttackReleaseCost();
    }
    public override float AttackRelease() {
        return trigger.AttackRelease();
    }

    public override void AttackCancel() {
        trigger.AttackCancel();
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