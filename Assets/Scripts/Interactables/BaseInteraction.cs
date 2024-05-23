using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteraction : MonoBehaviour
{
    public string menuName;

    public bool Continuous {get; protected set;} = false;
    public bool Blocking {get; protected set;} = false;

    protected AudioSource audioSource;

    protected virtual void Awake() {
        audioSource = GetComponentInParent<AudioSource>();
    }
    
    public abstract void Interact(CharacterHandler character);

    public abstract void Cancel();

    public virtual string MakeMenuName(ItemAction action) {
        return menuName;
    }
}
