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

    public void Highlight() {
        objectHandler.spriteRenderer.material.SetFloat("_DoOutline", 1.0f);
        objectHandler.spriteRenderer.material.SetColor("_Outline_Colour", Color.yellow);
    }
    public void Unhighlight() {
        objectHandler.spriteRenderer.material.SetFloat("_DoOutline", 0.0f);
    }
}
