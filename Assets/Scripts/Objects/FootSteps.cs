using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseBrain))]
[RequireComponent(typeof(AudioSource))]
public class FootSteps : MonoBehaviour
{
    BaseBrain brain;
    AudioSource audioSource;

    public float footStepInterval = 0.5f;
    float footStepTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        brain  = GetComponent<BaseBrain>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (brain.movement == Vector3.zero || brain.character.objectStatusHandler.isDodging) {
            footStepTimer = footStepInterval / 2f;
        } else {
            footStepTimer += Time.deltaTime;

            if (footStepTimer >= footStepInterval) {
                AudioManager.instance.PlayClipRandom(AudioID.FootStepDirt, audioSource);
                footStepTimer = 0f;
            }  
        }
    }
}
