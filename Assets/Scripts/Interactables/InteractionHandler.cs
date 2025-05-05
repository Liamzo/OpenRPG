using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ObjectHandler))]
public class InteractionHandler : MonoBehaviour
{
    public ObjectHandler objectHandler {get; private set;}

    List<BaseInteraction> interactions = new List<BaseInteraction>();

    void Awake()
    {
        GetComponentsInChildren<BaseInteraction>(interactions);
        objectHandler = GetComponent<ObjectHandler>();
    }

    public BaseInteraction GetInteraction(CharacterHandler character) {
        // For now just use the first interaction given
        return interactions[0];
    }

    public void Highlight() {
        objectHandler.GetRenderer().material.SetFloat("_DoOutline", 1.0f);
        objectHandler.GetRenderer().material.SetColor("_Outline_Colour", Color.yellow);
    }
    public void Unhighlight() {
        if (objectHandler.GetRenderer() == null) return;

        objectHandler.GetRenderer().material.SetFloat("_DoOutline", 0.0f);
    }
}
