using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talkable : BaseInteraction
{
    [SerializeField] private TextAsset inkJSON;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(CharacterHandler character) {
        DialogueHandler.GetInstance().EnterDialogueMode(inkJSON, GetComponentInParent<ObjectHandler>());
    }
}
