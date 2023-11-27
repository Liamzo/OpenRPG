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


    // Events
    public event System.Action<float> OnTrigger = delegate { };
    public void CallOnTrigger(float charge = 1f) { OnTrigger(charge); }
    
    public event System.Action<int> OnReload = delegate { };
    public void CallOnReload(int amount) { OnReload(amount); }

    public event System.Action OnAttack = delegate { };
    public void CallOnAttack() { OnAttack(); }

    public event System.Action<ObjectHandler> OnHitTarget = delegate { };
    public void CallOnHitTarget(ObjectHandler target) { OnHitTarget(target); }

    public ITrigger trigger;
    public IAttackType attackType;
    public IDamageType damageType;
    public IAnticipation anticipation {get; private set;}

    public Transform attackPoint;

    public int _canAttack = 0; // 0 = Can Attack. Add 1 when blocking, -1 when allowing
    bool _animationAttackLock = false;
    public void animationBlockAttack() {
        _animationAttackLock = true;
    }
    public void animationUnblockAttack() {
        _animationAttackLock = false;
    }

    protected virtual void Awake() {
        item = GetComponent<ItemHandler>();

        foreach (WeaponStatValue sv in baseWeaponStats.stats) {
            statsWeapon.Add(sv.statName, new Stat(sv.value));
        }


        // Handle cases where no Strategy is assigned
        trigger = strategies.GetComponent<ITrigger>();
        attackType = strategies.GetComponent<IAttackType>();
        damageType = strategies.GetComponent<IDamageType>();
        anticipation = strategies.GetComponent<IAnticipation>();
    }

    public float GetStatValue(WeaponStatNames statName) {
        return statsWeapon[statName].GetValue();
    }

    public void Holster() {
        transform.localPosition = new Vector3(0,0,0);
        transform.localRotation = Quaternion.identity;

        item.objectHandler.spriteRenderer.enabled = false;
    }
    public void Unholster() {
        transform.localPosition = new Vector3(0,0,0);
        transform.localRotation = Quaternion.identity;

        item.objectHandler.spriteRenderer.enabled = true;
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

    public float AttackHoldCost()
    {
        return trigger.AttackHoldCost();
    }
    public float AttackHold() {
        return trigger.AttackHold();
    }

    public float AttackReleaseCost()
    {
        return trigger.AttackReleaseCost();
    }
    public float AttackRelease() {
        return trigger.AttackRelease();
    }

    public void AttackCancel() {
        trigger.AttackCancel();
    }

    public void AttackAnticipation() {
        anticipation.AttackAnticipation();
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
    SelfKnockForce
}

[System.Serializable]
public struct WeaponStatValue {
    public WeaponStatNames statName;
    public float value;
}