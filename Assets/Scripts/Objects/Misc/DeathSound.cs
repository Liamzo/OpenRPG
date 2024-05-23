using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BaseBrain))]
public class DeathSound : MonoBehaviour
{
    BaseBrain brain;


    // Start is called before the first frame update
    void Start()
    {
        brain  = GetComponent<BaseBrain>();

        brain.character.OnDeath += OnDeath;
    }

    // Update is called once per frame
    void OnDeath(ObjectHandler attacker)
    {
        AudioManager.instance.PlayClipRandom(AudioID.Death, brain.character.audioSource);
    }
}

