using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThreatHandler : MonoBehaviour
{
    protected CharacterHandler characterHandler;
    protected FactionHandler factionHandler;


    // Events
    public event System.Action OnStartAttack = delegate { };
    public event System.Action OnReact = delegate { };

    public void CallOnStartAttack() {
        OnStartAttack();
    }

    protected void ReactToAttack() {
        OnReact();
    }
}
