using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open : BaseInteraction
{
    private void Awake() {
        Continuous = true;
    }
    
    public override void Interact(CharacterHandler character) {
        ContainerHandler.instance.OpenContainer(transform.parent.GetComponent<InventoryHandler>());
    }

    public override void Cancel()
    {
        ContainerHandler.instance.CloseContainer();
    }
}
