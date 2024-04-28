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


    public List<TriggerHolder> triggerHolders = new List<TriggerHolder> {new TriggerHolder(), new TriggerHolder()};

    // Events
    public event System.Action OnHolseter = delegate { };
    public void CallOnHolseter() { OnHolseter(); }
    public event System.Action<int> OnReload = delegate { };
    public void CallOnReload(int amount) { OnReload(amount); }

    public void CallOnTrigger(int triggerSlot, float charge = 1f) { triggerHolders[triggerSlot].CallOnTrigger(charge); }
    public void CallOnTriggerRelease(int triggerSlot, float charge = 1f) { triggerHolders[triggerSlot].CallOnTriggerRelease(charge); }
    
    public void CallOnAttack(int triggerSlot) { triggerHolders[triggerSlot].CallOnAttack(); }

    public void CallOnHitTarget(int triggerSlot, ObjectHandler target) { triggerHolders[triggerSlot].CallOnHitTarget(target); }

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


        // Set variables in trigger handlers
        triggerHolders = new List<TriggerHolder> {new TriggerHolder(), new TriggerHolder()};
        
        ITrigger[] triggers = strategies.GetComponents<ITrigger>();
        foreach (ITrigger trigger in triggers)
        {
            triggerHolders[((BaseStrategy)trigger).triggerSlot].trigger = trigger;
        }
        
        IAttackType[] attackTypes = strategies.GetComponents<IAttackType>();
        foreach (IAttackType attackType in attackTypes)
        {
            triggerHolders[((BaseStrategy)attackType).triggerSlot].attackType = attackType;
        }
        
        IDamageType[] damageTypes = strategies.GetComponents<IDamageType>();
        foreach (IDamageType damageType in damageTypes)
        {
            triggerHolders[((BaseStrategy)damageType).triggerSlot].damageType = damageType;
        }
        
        IAnticipation[] anticipations = strategies.GetComponents<IAnticipation>();
        foreach (IAnticipation anticipation in anticipations)
        {
            triggerHolders[((BaseStrategy)anticipation).triggerSlot].anticipation = anticipation;
        }
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

    public float AttackHoldCost(int triggerSlot)
    {
        return triggerHolders[triggerSlot].trigger.AttackHoldCost();
    }
    public bool CanAttackHold(int triggerSlot) {
        return triggerHolders[triggerSlot].trigger.CanAttackHold();
    }
    public void AttackHold(int triggerSlot) {
        triggerHolders[triggerSlot].trigger.AttackHold();
    }

    public float AttackReleaseCost(int triggerSlot)
    {
        return triggerHolders[triggerSlot].trigger.AttackReleaseCost();
    }
    public bool CanAttackRelease(int triggerSlot) {
        return triggerHolders[triggerSlot].trigger.CanAttackHold();
    }
    public void AttackRelease(int triggerSlot) {
        triggerHolders[triggerSlot].trigger.AttackRelease();
    }

    public void AttackCancel(int triggerSlot) {
        triggerHolders[triggerSlot].trigger.AttackCancel();
    }


    public void AttackAnticipation(int triggerSlot) {
        triggerHolders[triggerSlot].anticipation.AttackAnticipation();
    }


    void OnUnequip() {
        if (_animationAttackLock) {
            animationUnblockAttack();
        }
    }

}

[System.Serializable]
public class TriggerHolder {
    public event System.Action<float> OnTrigger = delegate { };
    public void CallOnTrigger(float charge = 1f) { OnTrigger(charge); }
    public event System.Action<float> OnTriggerRelease = delegate { };
    public void CallOnTriggerRelease(float charge = 1f) { OnTriggerRelease(charge); }

    public event System.Action OnAttack = delegate { };
    public void CallOnAttack() { OnAttack(); }

    public event System.Action<ObjectHandler> OnHitTarget = delegate { };
    public void CallOnHitTarget(ObjectHandler target) { OnHitTarget(target); }


    public ITrigger trigger;
    public IAttackType attackType;
    public IDamageType damageType;
    public IAnticipation anticipation;


    public float AttackHoldCost()
    {
        return trigger.AttackHoldCost();
    }
    public bool CanAttackHold() {
        return trigger.CanAttackHold();
    }
    public void AttackHold() {
        trigger.AttackHold();
    }

    public float AttackReleaseCost()
    {
        return trigger.AttackReleaseCost();
    }
    public bool CanAttackRelease() {
        return trigger.CanAttackHold();
    }
    public void AttackRelease() {
        trigger.AttackRelease();
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
    SelfKnockForce,
    Blocking
}

[System.Serializable]
public struct WeaponStatValue {
    public WeaponStatNames statName;
    public float value;
}