using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseBrain))]
public class YellTaunts : MonoBehaviour
{
    NonPlayerBrain brain;

    public float tauntCoolDown;
    public float tauntCoolDownRange;
    float tauntTimer;


    // Start is called before the first frame update
    void Start()
    {
        brain = GetComponent<NonPlayerBrain>();

        tauntTimer = tauntCoolDown + Random.Range(-tauntCoolDownRange, tauntCoolDownRange);
    }

    private void Update() {
        if (brain.threatHandler.Target != null) {
            tauntTimer -= Time.deltaTime;

            if (tauntTimer <= 0) {
                tauntTimer = tauntCoolDown + Random.Range(-tauntCoolDownRange, tauntCoolDownRange);

                AudioManager.instance.PlayClipRandom(AudioID.Taunt, brain.character.audioSource);
            }
        }
    }
}
