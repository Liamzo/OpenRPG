using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talkable : BaseInteraction
{
    ObjectHandler objectHandler;

    protected override void Awake() {
        base.Awake();
        
        Continuous = true;
        Blocking = true;

        objectHandler = GetComponentInParent<ObjectHandler>();
    }

    public override void Interact(CharacterHandler character) {
        DialogueHandler.GetInstance().EnterDialogueMode(objectHandler.baseStats.GetStatBlock<BaseCharacterStats>().dialogue, objectHandler);
        objectHandler.objectStatusHandler.BlockControls();
        objectHandler.objectStatusHandler.BlockMovementControls();

    }

    public override void Cancel()
    {
        DialogueHandler.GetInstance().ExitDialogue();
        objectHandler.objectStatusHandler.UnblockControls();
        objectHandler.objectStatusHandler.UnblockMovementControls();
    }
}
