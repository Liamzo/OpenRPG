using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : BaseInteraction
{
    public AudioID audioID;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Interact(CharacterHandler character) {
        AudioManager.instance.PlayClipRandom(AudioID.PickUp, character.audioSource);
        GetComponentInParent<ItemHandler>().PickUp(character);
    }

    public override void Cancel()
    {
        
    }
}
