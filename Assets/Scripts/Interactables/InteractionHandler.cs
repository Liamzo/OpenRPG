using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionHandler : MonoBehaviour
{
    List<BaseInteraction> interactions = new List<BaseInteraction>();

    void Awake()
    {
        GetComponentsInChildren<BaseInteraction>(interactions);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Use(CharacterHandler character) {
        // For now just use the first interaction given
        interactions[0].Interact(character);
    }
}
