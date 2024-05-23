using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open : BaseInteraction
{
    public AudioID audioID;

    protected override void Awake() {
        base.Awake();

        Continuous = true;
    }
    
    public override void Interact(CharacterHandler character) {
        ContainerHandler.instance.OpenContainer(GetComponentInParent<InventoryHandler>());
        AudioManager.instance.PlayClipRandom(audioID, audioSource);
    }

    public override void Cancel()
    {
        ContainerHandler.instance.CloseContainer();
    }
}
