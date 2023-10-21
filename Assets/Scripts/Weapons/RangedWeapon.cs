using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : BaseWeaponHandler
{
    public Transform attackPoint;

    public GameObject strategies;
    public ITrigger trigger;
    public IAttackType attackType;
    public IAmmo ammo;
    public IReload reload;
    public IFireRate fireRate;

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
        ammo = strategies.GetComponent<IAmmo>();
        reload = strategies.GetComponent<IReload>();
        fireRate = strategies.GetComponent<IFireRate>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (transform.eulerAngles.z >= 5.0f && transform.eulerAngles.z <= 175.0f) {
            item.objectHandler.spriteRenderer.transform.localEulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
        } else if (transform.eulerAngles.z <= 355.0f && transform.eulerAngles.z >= 185.0f) {
            item.objectHandler.spriteRenderer.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
        }

        reload.ReloadUpdate();
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


    public override void AttackAnticipation() {
    }
}
