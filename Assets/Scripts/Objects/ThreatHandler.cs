using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThreatHandler : MonoBehaviour
{
    protected CharacterHandler characterHandler;
    protected FactionHandler factionHandler;


    // Events
    public event System.Action<WeaponHandler, GameObject> OnStartAttack = delegate { };
    public event System.Action<WeaponHandler, GameObject> OnReact = delegate { };

    public void CallOnStartAttack(WeaponHandler weapon, GameObject projectile = null) {
        OnStartAttack(weapon, projectile);
    }

    protected void ReactToAttack(WeaponHandler weapon, GameObject projectile = null) {
        OnReact(weapon, projectile);
    }
}
