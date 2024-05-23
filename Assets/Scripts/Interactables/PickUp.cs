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
        GetComponentInParent<ItemHandler>().PickUp(character);
        AudioManager.instance.PlayClipRandom(audioID, audioSource);
    }

    public override void Cancel()
    {
        
    }
}
