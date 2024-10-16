using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

[System.Serializable]
public abstract class BaseStrategy : ScriptableObject
{
    protected WeaponHandler weapon;
    public string strategyName;
    public string strategyDescription;
    public Sprite strategyIcon;

    [SerializeField] public int triggerSlot = 0;

    // Start is called before the first frame update
    public virtual void Create(WeaponHandler weapon)
    {
        this.weapon = weapon;
    }

    public virtual void Delete() {
        
    }


    public virtual void Update() {

    }

    public virtual void LateUpdate() {
        
    }
}

public enum WeaponEvents
{
    OnTrigger,
    OnTriggerRelease,
    OnReload,
    OnAttack,
    OnHitTarget
}