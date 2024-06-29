using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseBrain))]
public class FootSteps : MonoBehaviour
{
    BaseBrain brain;

    public float footStepInterval = 0.5f;
    float footStepTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        brain  = GetComponent<BaseBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (brain.movement == Vector3.zero || brain.character.objectStatusHandler.isDodging) {
            footStepTimer = footStepInterval / 2f;
        } else {
            footStepTimer += Time.deltaTime;

            if (footStepTimer >= Mathf.Clamp(footStepInterval / (brain.movement.magnitude / brain.character.statsCharacter[CharacterStatNames.MovementSpeed].BaseValue), 0.2f, 1f)) {
                AudioManager.instance.PlayClipRandom(AudioID.FootStepDirt, brain.character.audioSource);
                footStepTimer = 0f;
            }  
        }
    }
}
