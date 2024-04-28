using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectHandler))]
[RequireComponent(typeof(ItemHandler))]
public class WeaponHandler : MonoBehaviour
{
    public ItemHandler item {get; private set;}

    public BaseWeaponStats baseWeaponStats;
    public Dictionary<WeaponStatNames, Stat> statsWeapon = new Dictionary<WeaponStatNames, Stat>();

    public Animator animator; // Probably used by everything when fully implemented
    public Transform _handle;
    public GameObject strategies;

    public bool Holstered {get; private set;}


    // Events
    public event System.Action OnHolseter = delegate { };
    public void CallOnHolseter() { OnHolseter(); }

    public event System.Action<float> OnPrimaryTrigger = delegate { };
    public void CallOnPrimaryTrigger(float charge = 1f) { OnPrimaryTrigger(charge); }
    public event System.Action<float> OnPrimaryTriggerRelease = delegate { };
    public void CallOnPrimaryTriggerRelease(float charge = 1f) { OnPrimaryTriggerRelease(charge); }
    
    public event System.Action<float> OnSecondaryTrigger = delegate { };
    public void CallOnSecondaryTrigger(float charge = 1f) { OnSecondaryTrigger(charge); }
    public event System.Action<float> OnSecondaryTriggerRelease = delegate { };
    public void CallOnSecondaryTriggerRelease(float charge = 1f) { OnSecondaryTriggerRelease(charge); }
    
    public event System.Action<int> OnReload = delegate { };
    public void CallOnReload(int amount) { OnReload(amount); }

    public event System.Action OnAttack = delegate { };
    public void CallOnAttack() { OnAttack(); }

    public event System.Action<ObjectHandler> OnHitTarget = delegate { };
    public void CallOnHitTarget(ObjectHandler target) { OnHitTarget(target); }

    public ITrigger triggerPrimary;
    public ITrigger triggerSecondary;
    public IAttackType attackType;
    public IDamageType damageType;
    public IAnticipation anticipation {get; private set;}

    public Transform attackPoint;

    public int _canAttack = 0; // 0 = Can Attack. Add 1 when blocking, -1 when allowing
    bool _animationAttackLock = false;
    public void animationBlockAttack() {
        _animationAttackLock = true;
        item.owner.objectStatusHandler.BlockControls();

        if (GetStatValue(WeaponStatNames.Blocking) > 0f)
            item.owner.objectStatusHandler.BlockMovementControls();
    }
    public void animationUnblockAttack() {
        _animationAttackLock = false;
        item.owner.objectStatusHandler.UnblockControls();
        if (GetStatValue(WeaponStatNames.Blocking) > 0f)
            item.owner.objectStatusHandler.UnblockMovementControls();
    }

    protected virtual void Awake() {
        item = GetComponent<ItemHandler>();

        item.OnUnequip += OnUnequip;

        foreach (WeaponStatValue sv in baseWeaponStats.stats) {
            statsWeapon.Add(sv.statName, new Stat(sv.value));
        }


        // Handle cases where no Strategy is assigned
        triggerPrimary = strategies.GetComponent<ITrigger>();
        attackType = strategies.GetComponent<IAttackType>();
        damageType = strategies.GetComponent<IDamageType>();
        anticipation = strategies.GetComponent<IAnticipation>();
    }

    public float GetStatValue(WeaponStatNames statName) {
        return statsWeapon[statName].GetValue();
    }

    public void Holster() {
        if (item.objectHandler.spriteRenderer.enabled == false)
            return;

        transform.localPosition = new Vector3(0,0,0);
        transform.localRotation = Quaternion.identity;

        item.objectHandler.spriteRenderer.enabled = false;

        Holstered = true;

        CallOnHolseter();
    }
    public void Unholster() {
        if (item.objectHandler.spriteRenderer.enabled == true)
            return;

        transform.localPosition = new Vector3(0,0,0);
        transform.localRotation = Quaternion.identity;

        item.objectHandler.spriteRenderer.enabled = true;

        Holstered = false;
    }


    public bool CanAttack() {
        return _canAttack == 0 && !_animationAttackLock;
    }

    public bool CanReload() {
        IAmmo ammo = strategies.GetComponent<IAmmo>();

        if (ammo == null) { return false; }

        if (ammo.GetCurrentAmmo() < ammo.GetMaxAmmo()) {
            return true;
        } else {
            return false;
        }
    }

    public float PrimaryAttackHoldCost()
    {
        return triggerPrimary.AttackHoldCost();
    }
    public bool PrimaryCanAttackHold() {
        return triggerPrimary.CanAttackHold();
    }
    public void PrimaryAttackHold() {
        triggerPrimary.AttackHold();
    }

    public float PrimaryAttackReleaseCost()
    {
        return triggerPrimary.AttackReleaseCost();
    }
    public bool PrimaryCanAttackRelease() {
        return triggerPrimary.CanAttackHold();
    }
    public void PrimaryAttackRelease() {
        triggerPrimary.AttackRelease();
    }

    public void PrimaryAttackCancel() {
        triggerPrimary.AttackCancel();
    }


    public float SecondaryAttackHoldCost()
    {
        return triggerSecondary.AttackHoldCost();
    }
    public bool SecondaryCanAttackHold() {
        return triggerSecondary.CanAttackHold();
    }
    public void SecondaryAttackHold() {
        triggerSecondary.AttackHold();
    }

    public float SecondaryAttackReleaseCost()
    {
        return triggerSecondary.AttackReleaseCost();
    }
    public bool SecondaryCanAttackRelease() {
        return triggerSecondary.CanAttackHold();
    }
    public void SecondaryAttackRelease() {
        triggerSecondary.AttackRelease();
    }

    public void SecondaryAttackCancel() {
        triggerSecondary.AttackCancel();
    }


    public void AttackAnticipation() {
        anticipation.AttackAnticipation();
    }


    void OnUnequip() {
        if (_animationAttackLock) {
            animationUnblockAttack();
        }
    }

}


[System.Serializable]
public enum WeaponStatNames {
    PenetrationValue,
    DamageRollCount,
    DamageRollValue,
    AttackTimer,
    Stagger,
    KnockBack,
    Range,
    Accuracy,
    ClipSize,
    ReloadDuration,
    StaminaCostEnd,
    StaminaCostHold,
    StaminaCostAim,
    SelfKnockForce,
    Blocking
}

[System.Serializable]
public struct WeaponStatValue {
    public WeaponStatNames statName;
    public float value;
}