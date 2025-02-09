using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Unity.VisualScripting;
using System.Linq;

[RequireComponent(typeof(ObjectHandler))]
[RequireComponent(typeof(ItemHandler))]
public class WeaponHandler : MonoBehaviour, ISaveable
{
    public ItemHandler item {get; private set;}

    private BaseWeaponStats baseWeaponStats;
    public Dictionary<WeaponStatNames, Stat> statsWeapon = new Dictionary<WeaponStatNames, Stat>();

    public Animator animator; // Probably used by everything when fully implemented
    public Transform _handle;
    
    public BoxCollider2D meleeHitbox;

    public bool Holstered {get; private set;}

    public Dictionary<WeaponModSlot, WeaponMod> mods = new Dictionary<WeaponModSlot, WeaponMod>();



    public List<TriggerHolder> triggerHolders = new List<TriggerHolder> {new TriggerHolder(), new TriggerHolder()};

    // Events
    public event System.Action OnHolseter = delegate { };
    public void CallOnHolseter() { OnHolseter(); }

    public event System.Action OnMod = delegate { };

    public event System.Action<int> OnReload = delegate { };
    public void CallOnReload(int amount) { OnReload(amount); }

    public void CallOnTrigger(int triggerSlot, float charge = 1f) { triggerHolders[triggerSlot].CallOnTrigger(charge); }
    public void CallOnTriggerRelease(int triggerSlot, float charge = 1f) { triggerHolders[triggerSlot].CallOnTriggerRelease(charge); }
    
    public void CallOnAttack(int triggerSlot) { triggerHolders[triggerSlot].CallOnAttack(); }

    public void CallOnHitTarget(int triggerSlot, ObjectHandler target, float charge, GameObject projectile = null) { triggerHolders[triggerSlot].CallOnHitTarget(target, this, charge, projectile); }

    public void CallAttackCancel(int triggerSlot) { }

    public Transform attackPoint;

    public int _canAttack = 0; // 0 = Can Attack. Add 1 when blocking, -1 when allowing
    bool _animationAttackLock = false;
    public void animationBlockAttack() {
        if (_animationAttackLock != false)
            return;

        _animationAttackLock = true;
        item.owner.objectStatusHandler.BlockControls();

        if (GetStatValue(WeaponStatNames.Blocking) > 0f)
            item.owner.objectStatusHandler.BlockMovementControls();
    }
    public void animationUnblockAttack() {
        if (_animationAttackLock != true)
            return;

        _animationAttackLock = false;
        item.owner.objectStatusHandler.UnblockControls();
        if (GetStatValue(WeaponStatNames.Blocking) > 0f)
            item.owner.objectStatusHandler.UnblockMovementControls();
    }



    protected virtual void Awake() {
        
    }

    private void Update() {
        foreach (BaseStrategy strategy in GetAllModStrategies()) {
            strategy.Update();
        }
    }

    private void LateUpdate() {
        foreach (BaseStrategy strategy in GetAllModStrategies()) {
            strategy.LateUpdate();
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
        IAmmo ammo = GetAllModStrategies().OfType<IAmmo>().FirstOrDefault();

        if (ammo == null) { return false; }

        if (ammo.GetCurrentAmmo() < ammo.GetMaxAmmo()) {
            return true;
        } else {
            return false;
        }
    }

    public float AttackHoldCost(int triggerSlot)
    {
        return triggerHolders[triggerSlot].AttackHoldCost();
    }
    public bool CanAttackHold(int triggerSlot) {
        return triggerHolders[triggerSlot].CanAttackHold();
    }
    public void AttackHold(int triggerSlot) {
        triggerHolders[triggerSlot].AttackHold();
    }

    public float AttackReleaseCost(int triggerSlot)
    {
        return triggerHolders[triggerSlot].AttackReleaseCost();
    }
    public bool CanAttackRelease(int triggerSlot) {
        return triggerHolders[triggerSlot].CanAttackRelease();
    }
    public void AttackRelease(int triggerSlot) {
        triggerHolders[triggerSlot].AttackRelease();
    }

    public void AttackCancel(int triggerSlot) {
        triggerHolders[triggerSlot].AttackCancel();
    }


    public void AttackAnticipation(int triggerSlot) {
        triggerHolders[triggerSlot].anticipation.AttackAnticipation();
    }


    void OnUnequip() {
        if (_animationAttackLock) {
            animationUnblockAttack();
        }
    }


    public List<BaseStrategy> GetAllModStrategies() {
        List<BaseStrategy> strategies = new List<BaseStrategy>();

        foreach (WeaponMod weaponMod in mods.Values)
        {
            if (weaponMod != null) {
                strategies.AddRange(weaponMod.strategies);
            }
        }

        return strategies;
    }

    public void ChangeMod(WeaponMod newMod) {
        WeaponMod oldMod = mods[newMod.modSlot];

        oldMod?.Delete();

        WeaponMod weaponMod = Instantiate(newMod);

        weaponMod.Create(this);

        mods[weaponMod.modSlot] = weaponMod;

        OnMod();
    }


    public void CreateBase()
    {
        item = GetComponent<ItemHandler>();

        item.OnUnequip += OnUnequip;


        baseWeaponStats = item.objectHandler.baseStats.GetStatBlock<BaseWeaponStats>();

        foreach (WeaponStatValue sv in baseWeaponStats.stats) {
            statsWeapon.Add(sv.statName, new Stat(sv.value));
        }

        // Set variables in trigger handlers
        triggerHolders = new List<TriggerHolder> {new TriggerHolder(), new TriggerHolder()};

        // Fill dictionary with all slots
        foreach (WeaponModSlot weaponModSlot in baseWeaponStats.neededModSlots)
        {
            mods.Add(weaponModSlot, null);
        }
        foreach (WeaponModSlot weaponModSlot in baseWeaponStats.optionalModSlots)
        {
            mods.Add(weaponModSlot, null);
        }

        // Add in the mods
        foreach (WeaponMod startingWeaponMod in baseWeaponStats.startingWeaponMods)
        {
            WeaponMod weaponMod = Instantiate(startingWeaponMod);

            weaponMod.Create(this);

            mods[weaponMod.modSlot] = weaponMod;
        }

        // TODO (LAZY): Check all needed mods slots are filled here




        List<ITrigger> triggers = GetAllModStrategies().OfType<ITrigger>().ToList();
        foreach (ITrigger trigger in triggers)
        {
            triggerHolders[((BaseStrategy)trigger).triggerSlot].trigger = trigger;
        }
        
        List<IAnticipation> anticipations = GetAllModStrategies().OfType<IAnticipation>().ToList();
        foreach (IAnticipation anticipation in anticipations)
        {
            triggerHolders[anticipation.triggerSlot].anticipation = anticipation;
        }

        animator.runtimeAnimatorController = baseWeaponStats.animationController;

        item.objectHandler.spriteRenderer.transform.eulerAngles = baseWeaponStats.spriteRotation;

        meleeHitbox.offset = new Vector2(baseWeaponStats.colliderSize.x, baseWeaponStats.colliderSize.y);
        meleeHitbox.size = new Vector2(baseWeaponStats.colliderSize.z, baseWeaponStats.colliderSize.w);

        attackPoint.localPosition = baseWeaponStats.attackPoint;
    }   

    public string SaveComponent()
    {
        string json = $"weapon: {{ mods: [";

        foreach (WeaponMod mod in mods.Values) {
            if (mod != null)
                json += mod.modId + ",";
        }

        return json + "]}";
    }

    public void LoadComponent(JSONNode data)
    {
        item = GetComponent<ItemHandler>();

        item.OnUnequip += OnUnequip;


        baseWeaponStats = item.objectHandler.baseStats.GetStatBlock<BaseWeaponStats>();

        foreach (WeaponStatValue sv in baseWeaponStats.stats) {
            statsWeapon.Add(sv.statName, new Stat(sv.value));
        }

        // Set variables in trigger handlers
        triggerHolders = new List<TriggerHolder> {new TriggerHolder(), new TriggerHolder()};

        // Fill dictionary with all slots
        foreach (WeaponModSlot weaponModSlot in baseWeaponStats.neededModSlots)
        {
            mods.Add(weaponModSlot, null);
        }
        foreach (WeaponModSlot weaponModSlot in baseWeaponStats.optionalModSlots)
        {
            mods.Add(weaponModSlot, null);
        }


        foreach (JSONNode node in data["weapon"]["mods"]) {
            WeaponMod mod = ModManager.Instance.FindModById(node);

            WeaponMod weaponMod = Instantiate(mod);

            weaponMod.Create(this);

            mods[weaponMod.modSlot] = weaponMod;
        }


        List<ITrigger> triggers = GetAllModStrategies().OfType<ITrigger>().ToList();
        foreach (ITrigger trigger in triggers)
        {
            triggerHolders[((BaseStrategy)trigger).triggerSlot].trigger = trigger;
        }
        
        List<IAnticipation> anticipations = GetAllModStrategies().OfType<IAnticipation>().ToList();
        foreach (IAnticipation anticipation in anticipations)
        {
            triggerHolders[anticipation.triggerSlot].anticipation = anticipation;
        }
    }




    
    // This probably isn't the best, think of a smarter way
    public void RunCoroutine(IEnumerator coroutine) {
        StartCoroutine(coroutine);
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

    public event System.Action<ObjectHandler, HitOutcome, float, GameObject> OnHitTarget = delegate { };
    public void CallOnHitTarget(ObjectHandler target, WeaponHandler weapon, float charge, GameObject projectile = null) {
        HitOutcome hitOutcome;
        if (target != null)
            hitOutcome = target.GetHit(weapon, (CharacterHandler) weapon.item.owner, projectile);
        else
            hitOutcome = HitOutcome.Hit;

        OnHitTarget(target, hitOutcome, charge, projectile);
    }

    public event System.Action OnAttackCancel = delegate { };


    public ITrigger trigger;
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
        return trigger.CanAttackRelease();
    }
    public void AttackRelease() {
        trigger.AttackRelease();
    }

    public void AttackCancel() {
        OnAttackCancel();
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
    AttackSpeed,
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
    Blocking,
    AmmoCost
}

[System.Serializable]
public struct WeaponStatValue {
    public WeaponStatNames statName;
    public float value;

    public WeaponStatValue(WeaponStatNames statName, float value) {
        this.statName = statName;
        this.value = value;
    }
}