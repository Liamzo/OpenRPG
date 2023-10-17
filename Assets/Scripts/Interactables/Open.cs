using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open : BaseInteraction
{
    public override void Interact(CharacterHandler character) {
        ContainerHandler.instance.OpenContainer(transform.parent.GetComponent<InventoryHandler>());
    }
}
